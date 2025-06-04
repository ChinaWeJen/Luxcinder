import os
import re
import sys

def capitalize_words(name):
    """智能识别并大写复合词中的独立单词"""
    # 特殊词汇处理字典
    SPECIAL_WORDS = {
        'pickaxe': 'PickAxe',
        'broadsword': 'BroadSword',
        'shortsword': 'ShortSword',
        'waraxe': 'WarAxe'
    }
    
    # 处理常见分隔符：下划线、连字符、空格
    parts = re.split(r'([0-9]+|_|-|\s+)', name.lower())
    result = []
    for part in parts:
        if part.isdigit():
            result.append(part)  # 保留数字不变
        elif part in ['_', '-', ' ']:
            result.append(part)  # 保留分隔符
        else:
            # 检查是否是特殊词汇
            if part in SPECIAL_WORDS:
                result.append(SPECIAL_WORDS[part])
                continue
                
            # 智能分割复合词（处理驼峰式、连续字母等情况）
            sub_words = re.findall(r'([a-z]+|[A-Z][a-z]+)', part)
            if not sub_words:
                sub_words = re.findall(r'([a-z]+)', part)
                
            for word in sub_words:
                # 处理连续辅音字母（如"iron"中的"ir"和"on"）
                if len(word) > 3 and not re.search(r'[aeiouy]', word[1:]):
                    syllables = re.findall(r'([^aeiouy]*[aeiouy]?)', word)
                    syllables = [s for s in syllables if s]
                    if len(syllables) > 1:
                        for syl in syllables:
                            if syl:
                                result.append(syl.capitalize())
                        continue
                
                # 普通单词处理
                if word.isupper() and len(word) > 1:
                    result.append(word)  # 保留全大写的缩写
                else:
                    result.append(word.capitalize())
    return ''.join(result)

def process_items_directory(root_dir):
    """处理Items目录下的所有文件和文件夹，保持文件与类名同步"""
    items_dir = os.path.join(root_dir, 'Content', 'Items')
    if not os.path.exists(items_dir):
        print(f"错误: Items目录不存在 - {items_dir}")
        return

    print(f"处理目录: {items_dir}")
    print("将执行以下操作:")
    print("1. 修改.cs文件中的类名")
    print("2. 重命名对应的.png素材文件")
    print("3. 更新文件夹名称")
    
    # 文件扩展名白名单
    EXTENSIONS = ('.png', '.cs', '.json', '.fx')
    
    # 先处理.cs文件更新类名
    class_mappings = {}
    for root, _, files in os.walk(items_dir):
        for file in files:
            if file.endswith('.cs'):
                file_path = os.path.join(root, file)
                update_class_name(file_path, class_mappings)
    
    # 然后处理素材文件和目录
    for root, _, files in os.walk(items_dir):
        for file in files:
            if file.endswith(EXTENSIONS):
                process_related_files(root, file, class_mappings)
    
    for root, dirs, _ in os.walk(items_dir, topdown=False):
        for dir_name in dirs:
            process_item(os.path.join(root, dir_name))

def update_class_name(file_path, class_mappings):
    """更新.cs文件中的类名并记录映射关系"""
    with open(file_path, 'r+', encoding='utf-8') as f:
        content = f.read()
        
        # 匹配类定义
        pattern = r'public class (\w+) : (ModItem|ModNPC|ModTile|ModProjectile)'
        matches = re.findall(pattern, content)
        
        if matches:
            for old_name, base_class in matches:
                new_name = capitalize_words(old_name)
                if old_name != new_name:
                    print(f"更新类名: {old_name} -> {new_name}")
                    content = content.replace(
                        f'public class {old_name} : {base_class}',
                        f'public class {new_name} : {base_class}'
                    )
                    # 记录类名映射关系
                    class_mappings[old_name.lower()] = new_name
            
            # 写回文件
            f.seek(0)
            f.write(content)
            f.truncate()

def process_related_files(root, file, class_mappings):
    """处理与类名关联的素材文件"""
    name, ext = os.path.splitext(file)
    lower_name = name.lower()
    
    # 检查是否有对应的类名映射
    if lower_name in class_mappings:
        new_name = class_mappings[lower_name] + ext
        old_path = os.path.join(root, file)
        new_path = os.path.join(root, new_name)
        
        if os.path.exists(new_path):
            print(f"警告: 跳过 {old_path} -> {new_path} (目标已存在)")
            return
        
        print(f"重命名素材文件: {file} -> {new_name}")
        os.rename(old_path, new_path)
    else:
        # 普通文件处理
        process_item(os.path.join(root, file))

def process_item(path):
    """处理单个文件或目录"""
    dirname, basename = os.path.split(path)
    name, ext = os.path.splitext(basename)
    
    new_name = capitalize_words(name) + ext
    if new_name == basename:
        return  # 无需修改
    
    new_path = os.path.join(dirname, new_name)
    
    if os.path.exists(new_path):
        print(f"警告: 跳过 {path} -> {new_path} (目标已存在)")
        return
    
    print(f"重命名: {path} -> {new_path}")
    os.rename(path, new_path)

if __name__ == "__main__":
    mod_dir = os.path.dirname(os.path.abspath(__file__))
    print("物品名称格式化工具")
    print("将处理Content/Items目录下的所有文件和文件夹")
    print("示例: iron_sword.png -> IronSword.png")
    print("      ancientGoldSword.cs -> AncientGoldSword.cs")
    
    confirm = input("确认执行? (y/n): ").lower()
    if confirm != 'y':
        print("操作已取消")
        sys.exit(0)
    
    try:
        process_items_directory(mod_dir)
        print("操作完成!")
    except Exception as e:
        print(f"错误: {e}")
        print("部分修改可能已完成，请检查文件系统")