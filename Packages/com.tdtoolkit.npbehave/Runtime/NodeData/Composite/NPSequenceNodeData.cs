using System;

namespace NPBehave
{
    [Serializable]
    public class NPSequenceNodeData : ANPNodeDataBase
    {
        [NonSerialized]
        private Sequence m_SequenceNode;

        public override Node GetNode()
        {
            return this.m_SequenceNode;
        }

        public override Composite CreateComposite(Node[] nodes)
        {
            this.m_SequenceNode = new Sequence(nodes);
            return this.m_SequenceNode;
        }
    }
}