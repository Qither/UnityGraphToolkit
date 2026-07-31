using GraphProcessor;
using NPBehave;

namespace NPBehaveEditor
{
    [System.Serializable]
    [NodeMenuItem("NPBehave/Composite/Sequence", typeof(NPBehaveGraph))]
    public class NPSequenceNode : ANPCompositeNodeBase
    {
        public override string Name => "Sequence";
        public override string Icon => "DarkSequenceIcon";

        public NPSequenceNodeData SequenceNodeData = new NPSequenceNodeData() { nodeDes = "Sequence" };

        public override ANPNodeDataBase GetNodeData()
        {
            return this.SequenceNodeData;
        }
    }
}
