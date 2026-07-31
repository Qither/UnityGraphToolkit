using GraphProcessor;
using NPBehave;

namespace NPBehaveEditor
{
    [NodeMenuItem("NPBehave/Decorator/Repeater", typeof(NPBehaveGraph))]
    public class NPRepeaterNode : ANPDecoratorNodeBase
    {
        public override string name => "Repeater";
        public override string Icon => "DarkRepeaterIcon";
        
        public NPRepeaterNodeData RepeaterNodeData = new NPRepeaterNodeData() { nodeDes = "Repeater" };

        public override ANPNodeDataBase GetNodeData()
        {
            return this.RepeaterNodeData;
        }
    }
}
