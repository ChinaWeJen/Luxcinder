using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.NPCChat.Nodes;

namespace Luxcinder.Functions.NPCChat
{
    /// <summary>
    /// NPC对话流程控制
    /// </summary>
    public class NPCChatControlFlow
    {
        private NPCChatNode _current;
        public NPCChatNode Current => _current;

        public NPCChatControlFlow()
        {
        }

        /// <summary>
        /// 启动对话流程
        /// </summary>
        public void Start(NPCChatNode start)
        {
            _current = start;
        }

        /// <summary>
        /// 更新流程，deltaTime为秒
        /// </summary>
        public void Update()
        {
            if (_current == null)
                return;

            _current.Update();
            if (_current.Next != null && _current.CanMoveNext)
            {
                _current = _current.Next; 
            }
        }

        /// <summary>
        /// 玩家选择某个选项
        /// </summary>
        public void SelectOption(int index)
        {
            _current.UserChooseOption(index);
        }
    }
}
