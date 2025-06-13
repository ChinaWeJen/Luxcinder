using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Luxcinder.Functions.NPCChat.Nodes;
internal class ExhaustiveOptionsNode : NPCChatNode
{
    public override NPCChatPage PageInfo
    {
        get
        {
            var optionTexts = _options.Select(o => o.Item1()).ToList(); // 提取选项文本
            if (_currentNode == null)
            {
                return new NPCChatPage(_text(), optionTexts);
            }
            return _currentNode.Next == null
                ? new NPCChatPage(_currentNode.PageInfo.Text, optionTexts)
                : _currentNode.PageInfo; // 如果有下一个节点，则返回当前节点的页面信息
        }
    }


    private Func<string> _text;
    private List<(Func<string>, NPCChatNode)> _options = new List<(Func<string>, NPCChatNode)>();
    private bool _canMoveNext = false;
    private NPCChatNode _currentNode = null;

    public ExhaustiveOptionsNode(Func<string> text, List<(Func<string>, NPCChatNode)> options)
    {
        _text = text;
        _options = options;
    }

    public override bool CanMoveNext
    {
        get
        {
            return _canMoveNext;
        }
    }

    public override void Update()
    {
        if (_currentNode != null)
        {
            _currentNode.Update();
            if (_currentNode.Next == null && _currentNode.CanMoveNext)
            {
                _canMoveNext = true;
            }
            else if(_currentNode.Next != null && _currentNode.CanMoveNext)
            {
                _currentNode = _currentNode.Next;
            }
        }
    }


    public override void UserChooseOption(int index)
    {
        if (_currentNode == null && index != -1)
        {
            _currentNode = _options[index].Item2;
            _options.RemoveAt(index); // 移除已选择的选项
        }
        else if (_currentNode != null)
        {
            _currentNode.UserChooseOption(index);
            if (_currentNode.Next == null)
            {
                if (_options.Count > 0)
                {
                    // 如果还有选项，那么继续
                    _currentNode = _options[index].Item2;
                    _options.RemoveAt(index); // 移除已选择的选项
                }
                else
                {
                    // 如果没有选项了，什么都不做，等待下一步
                }
            }
            else
            {
                _currentNode = _currentNode.Next; // 否则继续到下一个节点
            }
            
        }
    }
}
