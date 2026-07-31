using GraphProcessor;
using NPBehave;
using NPBehaveEditor;
using ToolkitDemo.NPBehaveDemo;

namespace ToolkitDemo.Editor
{
    [System.Serializable]
    [NodeMenuItem("NPBehave/Task/Demo Set Blackboard", typeof(NPBehaveGraph))]
    public sealed class DemoSetBlackboardActionNode : ANPTaskNodeBase
    {
        public override string name => "Set Blackboard";

        public override string Icon => "DarkActionIcon";

        [ShowInInspector]
        public NPActionNodeData ActionNodeData = new NPActionNodeData
        {
            nodeDes = "Increment DemoRunCount",
            actionData = new DemoSetBlackboardAction()
        };

        protected override void Enable()
        {
            if (this.graph.nodes.Count > 0 &&
                this.graph.nodes[0] is NPRootNode { RootNodeData: { } } rootNode)
            {
                this.ActionNodeData.Setup(rootNode.RootNodeData);
            }
        }

        public override ANPNodeDataBase GetNodeData()
        {
            return this.ActionNodeData;
        }
    }
}
