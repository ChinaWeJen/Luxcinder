//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Luxcinder.Functions.NPCChat.ChatNodes
//{
//    public class NPCChatLoopBackAllOptionsParagraph : NPCChatNode
//    {
//        private List<(Func<string>, NPCChatNode)> _options;
//        private int _initalOptionsCount;
//        public NPCChatLoopBackAllOptionsParagraph(Func<string> func, List<(Func<string>, NPCChatNode)> options) : base(func)
//        {
//            _options = options;
//            _initalOptionsCount = options.Count;
//            foreach (var (option, para) in options)
//            {
//                para.Next = this; // 设置每个选项的下一段落为当前段落
//            }
//        }
//        public override bool ImmediateShow => _options.Count < _initalOptionsCount;

//        public override List<NPCChatOption> Options
//        {
//            get
//            {
//                if (_options == null || _options.Count == 0)
//                    return null;
//                return _options.Select(option => new NPCChatOption(option.Item1, option.Item2)).ToList();
//            }
//        }

//        public override void UserChooseOption(int index)
//        {
//            // 移除选择了的选项
//            _options.RemoveAt(index);

//            if (_options.Count == 1)
//            {
//                _options[0].Item2.Next = Next;
//            }
//        }
//    }

//}
