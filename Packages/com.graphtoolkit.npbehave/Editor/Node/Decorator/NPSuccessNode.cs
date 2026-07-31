using GraphProcessor;
using NPBehave;

namespace NPBehaveEditor
{
    [NodeMenuItem("NPBehave/Decorator/Success", typeof(NPBehaveGraph))]
    public class NPSuccessNode : ANPDecoratorNodeBase
    {
        public override string name => "Success";
        public override string Icon => "DarkUntilSuccessIcon";
        
        public NPSuccessNodeData SuccessNodeData = new NPSuccessNodeData() { nodeDes = "Success" };

        public override ANPNodeDataBase GetNodeData()
        {
            return this.SuccessNodeData;
        }
    }
}
