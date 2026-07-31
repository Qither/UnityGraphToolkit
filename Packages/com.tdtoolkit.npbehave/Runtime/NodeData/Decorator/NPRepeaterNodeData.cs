using System;

namespace NPBehave
{
    [Serializable]
    public class NPRepeaterNodeData : ANPNodeDataBase
    {
        [NonSerialized]
        public Repeater Repeater;

        public override Node GetNode()
        {
            return this.Repeater;
        }
        
        public override Decorator CreateDecoratorNode(RuntimeTree runtimeTree, Node node)
        {
            this.Repeater = new Repeater(node);
            return this.Repeater;
        }

    }
}