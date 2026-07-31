using System;

namespace NPBehave
{
    [Serializable]
    public class NPSuccessNodeData : ANPNodeDataBase
    {
        [NonSerialized]
        public Success Success;
        
        public override Node GetNode()
        {
            return this.Success;
        }
        
        public override Decorator CreateDecoratorNode(RuntimeTree runtimeTree, Node node)
        {
            this.Success = new Success(node);
            return this.Success;
        }
    }
}