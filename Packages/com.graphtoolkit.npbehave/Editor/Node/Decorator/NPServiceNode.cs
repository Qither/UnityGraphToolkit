using GraphProcessor;
using NPBehave;

namespace NPBehaveEditor
{
    [NodeMenuItem("NPBehave/Decorator/Service", typeof(NPBehaveGraph))]
    public class NPServiceNode : ANPDecoratorNodeBase
    {
        public override string name => "Service";
        public override string Icon => "DarkCoolDownIcon";

        [ShowInInspector]
        public NPServiceNodeData ServiceNodeData = new NPServiceNodeData() { nodeDes = "Service" };

        protected override void Enable()
        {
            if (this.graph.nodes[0] is NPRootNode { RootNodeData: { } } rootNode)
            {
                this.ServiceNodeData.Setup(rootNode.RootNodeData);
            }
        }
        
        public override ANPNodeDataBase GetNodeData()
        {
            return this.ServiceNodeData;
        }
    }
}
