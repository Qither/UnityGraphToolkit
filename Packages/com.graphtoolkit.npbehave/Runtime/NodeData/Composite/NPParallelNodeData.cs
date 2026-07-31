using System;

namespace NPBehave
{
    [Serializable]
    public class NPParallelNodeData : ANPNodeDataBase
    {
        [NonSerialized]
        private Parallel m_ParallelNode;
        
        public Parallel.Policy successPolicy = Parallel.Policy.All;
        
        public Parallel.Policy failurePolicy = Parallel.Policy.All;
        
        public override Composite CreateComposite(Node[] nodes)
        {
            this.m_ParallelNode = new Parallel(this.successPolicy, this.failurePolicy, nodes);
            return this.m_ParallelNode;
        }

        public override Node GetNode()
        {
            return this.m_ParallelNode;
        }
    }
}