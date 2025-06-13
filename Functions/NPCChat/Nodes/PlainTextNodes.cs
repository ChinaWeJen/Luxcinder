using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luxcinder.Functions.NPCChat.Nodes;
public class PlainText : NPCChatNode
{
	public override NPCChatPage PageInfo
    {
        get
        {
            return new NPCChatPage(_text(), null);
        }
    }
	public override bool CanMoveNext => _canMoveNext;

    private Func<string> _text;
    private bool _canMoveNext = false;

    public PlainText(Func<string> text)
    {
        _text = text;
    }

	public override void UserChooseOption(int index) => _canMoveNext = true;
}


public class PlainTextWithOptions : NPCChatNode
{
    public override NPCChatPage PageInfo
    {
        get
        {
            var optionTexts = _options.Select(o => o.Item1()).ToList(); // 提取选项文本
            return new NPCChatPage(_text(), optionTexts);
        }
    }


    private Func<string> _text;
    private List<(Func<string>, NPCChatNode)> _options = new List<(Func<string>, NPCChatNode)>();
    private bool _canMoveNext = false;

    public PlainTextWithOptions(Func<string> text, List<(Func<string>, NPCChatNode)> options)
    {
        _text = text;
        _options = options;
    }


    public override void UserChooseOption(int index)
    {
        if (index != -1)
        {
            _canMoveNext = true;
            Next = _options[index].Item2;
        }
    }
}
