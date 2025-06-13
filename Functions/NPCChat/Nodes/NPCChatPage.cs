using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luxcinder.Functions.NPCChat.Nodes;

/// <summary>
/// 表示NPC对话的一个页面或段落，包含需要显示的文本、选项、渲染内容等信息
/// </summary>
public class NPCChatPage
{
    /// <summary>
    /// 页面显示的文本内容
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// 页面可供选择的选项列表
    /// </summary>
    public List<string> Options { get; set; }

    /// <summary>
    /// 是否立即显示该页面，不使用打字机效果
    /// </summary>
    public bool ImmediateShow { get; set; }

    /// <summary>
    /// 打字机每个字的打字间隔时间，单位为帧
    /// </summary>
    public int TypewriterTypeInterval { get; set; }

    /// <summary>
    /// 需不需要用户点击下一步才能继续对话
    /// </summary>
    public bool NeedUserClickNextStep { get; set; }

    public NPCChatPage(string text, List<string> options)
    {
        Text = text;
        Options = options;
        ImmediateShow = false;
        TypewriterTypeInterval = 7;
        NeedUserClickNextStep = true; // 默认需要用户点击下一步
    }
}
