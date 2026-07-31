using System;

namespace NPBehave
{
    [Serializable]
    public class NPFailureNodeData : ANPNodeDataBase
    {
        [NonSerialized]
        public Failure Failure;
        
        public override Node GetNode()
        {
            return this.Failure;
        }
        
        public override Decorator CreateDecoratorNode(RuntimeTree runtimeTree, Node node)
        {
            this.Failure = new Failure(node);
            return this.Failure;
        }
    }
}