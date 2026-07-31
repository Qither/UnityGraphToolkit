using GraphProcessor;
using NPBehave;
using System.Runtime.CompilerServices;

namespace NPBehaveEditor
{
    [NodeMenuItem("NPBehave/Decorator/BlackboardCondition", typeof(NPBehaveGraph))]
    public class NPBlackboardConditionNode : ANPDecoratorNodeBase
    {
        public override string Name => "BCondition";
        public override string Icon => "DarkConditionalIcon";

        [ShowInInspector]
        public NPBlackboardConditionNodeData BlackboardConditionNodeData = new NPBlackboardConditionNodeData() { nodeDes = "BlackboardCondition" };

        protected override void Enable()
        {
            if (this.graph.nodes[0] is NPRootNode { RootNodeData: { } } rootNode)
            {
                this.BlackboardConditionNodeData.condition.RootNodeDataData = rootNode.RootNodeData;
            }
        }

        protected override void Disable()
        {
            this.BlackboardConditionNodeData.condition.RootNodeDataData = null;
        }

        public override ANPNodeDataBase GetNodeData()
        {
            return this.BlackboardConditionNodeData;
        }
    }
}