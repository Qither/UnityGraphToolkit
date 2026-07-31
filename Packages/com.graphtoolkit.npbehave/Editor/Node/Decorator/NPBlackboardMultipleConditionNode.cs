using GraphProcessor;
using NPBehave;

namespace NPBehaveEditor
{
    [NodeMenuItem("NPBehave/Decorator/BlackboardMultipleCondition", typeof(NPBehaveGraph))]
    public class NPBlackboardMultipleConditionNode : ANPDecoratorNodeBase
    {
        public override string name => "BMCondition";
        public override string Icon => "DarkConditionalEvaluatorIcon";

        [ShowInInspector]
        public NPBlackboardMultipleConditionsNodeData BlackboardMultipleConditionNodeData = new NPBlackboardMultipleConditionsNodeData() { nodeDes = "BlackboardMultipleCondition" };

        protected override void Enable()
        {
            if (!(this.graph.nodes[0] is NPRootNode { RootNodeData: { } } rootNode)) return;

            this.BlackboardMultipleConditionNodeData.RootNodeData = rootNode.RootNodeData;
            
            if (this.BlackboardMultipleConditionNodeData.matchInfos == null || this.BlackboardMultipleConditionNodeData.matchInfos.Count == 0) return;
            
            foreach (MatchInfo matchInfo in this.BlackboardMultipleConditionNodeData.matchInfos)
            {
                matchInfo.blackBoardHandleData.RootNodeDataData = rootNode.RootNodeData;
            }
        }

        protected override void Disable()
        {
            this.BlackboardMultipleConditionNodeData.RootNodeData = null;
            
            if (this.BlackboardMultipleConditionNodeData.matchInfos == null || this.BlackboardMultipleConditionNodeData.matchInfos.Count == 0) return;
            foreach (MatchInfo matchInfo in this.BlackboardMultipleConditionNodeData.matchInfos)
            {
                matchInfo.blackBoardHandleData.RootNodeDataData = null;
            }
        }
        
        public override ANPNodeDataBase GetNodeData()
        {
            return this.BlackboardMultipleConditionNodeData;
        }
    }
}
