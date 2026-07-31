using System;

namespace NPBehave
{
    [Serializable]
    public class NPSelectorNodeData : ANPNodeDataBase
    {
        [NonSerialized]
        private Selector m_SelectorNode;
        
        public override Composite CreateComposite(Node[] nodes)
        {
            this.m_SelectorNode = new Selector(nodes);
            return this.m_SelectorNode;
        }

        public override Node GetNode()
        {
            return this.m_SelectorNode;
        }
    }
}