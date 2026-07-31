using GraphProcessor;
using NPBehave;

namespace NPBehaveEditor
{
    // [NodeMenuItem("NPBehave/Task/Wait", typeof(NPBehaveGraph))]
    public class NPWaitNode : ANPTaskNodeBase
    {
        public override string name => "Wait";
        public override string Icon => "DarkWaitIcon";

        [ShowInInspector]
        public NPWaitNodeData WaitNodeData = new NPWaitNodeData() { nodeDes = "Wait" };

        protected override void Enable()
        {
            if (this.graph.nodes[0] is NPRootNode { RootNodeData: { } } rootNode)
            {
                this.WaitNodeData.blackBoardHandleData.RootNodeDataData = rootNode.RootNodeData;
                this.WaitNodeData.blackBoardHandleData.KeyType          = typeof(float);
                this.WaitNodeData.blackBoardHandleData.IsLink           = true;
            }
        }

        protected override void Disable()
        {
            this.WaitNodeData.blackBoardHandleData.RootNodeDataData = null;
        }
        
        public override ANPNodeDataBase GetNodeData()
        {
            return this.WaitNodeData;
        }
    }
}
