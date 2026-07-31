using GraphProcessor;
using NPBehave;

namespace NPBehaveEditor
{
    [NodeMenuItem("NPBehave/Decorator/Failure", typeof(NPBehaveGraph))]
    public class NPFailureNode : ANPDecoratorNodeBase
    {
        public override string Name => "Failure";
        public override string Icon => "DarkUntilFailureIcon";
        
        public NPFailureNodeData FailureNodeData = new NPFailureNodeData() { nodeDes = "Failure" };

        public override ANPNodeDataBase GetNodeData()
        {
            return this.FailureNodeData;
        }
    }
}