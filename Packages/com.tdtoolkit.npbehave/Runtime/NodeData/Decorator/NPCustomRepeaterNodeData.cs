using System;

namespace NPBehave
{
    [Serializable]
    public class NPCustomRepeaterNodeData : ANPNodeDataBase
    {
        [NonSerialized]
        public CustomRepeater CustomRepeater;

        public override Node GetNode()
        {
            return this.CustomRepeater;
        }
        
        public override Decorator CreateDecoratorNode(RuntimeTree runtimeTree, Node node)
        {
            this.CustomRepeater = new CustomRepeater(node);
            return this.CustomRepeater;
        }

    }
}