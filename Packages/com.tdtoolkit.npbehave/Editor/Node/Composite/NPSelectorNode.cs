using GraphProcessor;
using NPBehave;

namespace NPBehaveEditor
{
    [NodeMenuItem("NPBehave/Composite/Selector", typeof(NPBehaveGraph))]
    public class NPSelectorNode : ANPCompositeNodeBase
    {
        public override string Name => "Selector";
        public override string Icon => "DarkSelectorIcon";
        
        public NPSelectorNodeData SelectorNodeData = new NPSelectorNodeData() { nodeDes = "Selector" };

        public override ANPNodeDataBase GetNodeData()
        {
            return this.SelectorNodeData;
        }
    }
}