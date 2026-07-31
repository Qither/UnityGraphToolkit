using System;

namespace RedDotSystem.Runtime
{
    [Serializable]
    public abstract class RedDotRule : IRedDotRule
    {
        [NonSerialized]
        private TrieNode<(RedDotData nodeData, int nodeValue)> m_Order;
        public TrieNode<(RedDotData nodeData, int nodeValue)> Order => this.m_Order;
        
        [NonSerialized]
        private RedDotService m_RedDotService;
        public RedDotService RedDotService => this.m_RedDotService;

        public void Setup(RedDotService service, TrieNode<(RedDotData nodeData, int nodeValue)> node)
        {
            this.m_Order         = node;
            this.m_RedDotService = service;
            
            this.OnStart();
        }

        public virtual void OnStart()
        {
        }

        public void Execute(TrieNode<(RedDotData nodeData, int nodeValue)> executeNode, RedDotExecuteDelegate nodeExecute)
        {
            this.OnExecuteBefore(executeNode);
            if (this.IsExecute())
            {
                nodeExecute?.Invoke(this.Order);
            }
            this.OnExecuteAfter(executeNode);
        }

        protected virtual void OnExecuteAfter(TrieNode<(RedDotData nodeData, int nodeValue)> node)
        {
        }

        protected virtual void OnExecuteBefore(TrieNode<(RedDotData nodeData, int nodeValue)> node)
        {
        }
        
        protected virtual bool IsExecute()
        {
            return true;
        }
    }
}