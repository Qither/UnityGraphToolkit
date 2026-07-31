using GraphProcessor;
using NPBehave;

namespace NPBehaveEditor
{
    [NodeMenuItem("NPBehave/Decorator/CustomRepeater", typeof(NPBehaveGraph))]
    public class NPCustomRepeaterNode : ANPDecoratorNodeBase
    {
        public override string Name => "CustomRepeater";
        public override string Icon => "DarkRepeaterIcon";
        
        public NPCustomRepeaterNodeData CustomRepeaterNodeData = new NPCustomRepeaterNodeData() { nodeDes = "CustomRepeater" };

        public override ANPNodeDataBase GetNodeData()
        {
            return this.CustomRepeaterNodeData;
        }
    }
}