using GraphProcessor;
using NPBehave;
using System.Runtime.CompilerServices;

namespace NPBehaveEditor
{
    [NodeMenuItem("NPBehave/Composite/Parallel", typeof(NPBehaveGraph))]
    public class NPParallelNode : ANPCompositeNodeBase
    {
        public override string Name => "Parallel";
        public override string Icon => "DarkParallelIcon";
        
        [ShowInInspector]
        public NPParallelNodeData ParallelNodeData = new NPParallelNodeData() { nodeDes = "Parallel" };

        public override ANPNodeDataBase GetNodeData()
        {
            return this.ParallelNodeData;
        }
    }
}