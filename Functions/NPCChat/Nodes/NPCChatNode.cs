using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luxcinder.Functions.NPCChat.Nodes;

/// <summary>
/// 对话节点类
/// </summary>
public abstract class NPCChatNode
{
    /// <summary>
    /// 段落文本
    /// </summary>
    public abstract NPCChatPage PageInfo
	{
		get;
	}

    /// <summary>
    /// 下一个段落（可为null，此时控制流会停在这个节点）
    /// 默认当用户点击下一步时，制定下一步节点，否则等待
    /// </summary>
    public NPCChatNode Next
    {
        get; set;
    }

    /// <summary>
    /// 是否下一帧可以触发新的页面
    /// </summary>
    public virtual bool CanMoveNext
    {
        get
        {
            return false;
        }
    }

    /// <summary>
    /// 当用户选择了某个选项时调用，如果是当用户点击下一步时调用，index一定为0
    /// </summary>
    /// <param name="index"></param>
    public virtual void UserChooseOption(int index)
	{
	}


    /// <summary>
    /// 每帧更新
    /// </summary>
	public virtual void Update()
	{
    }
}