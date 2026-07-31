using GraphProcessor;
using NPBehave;

namespace NPBehaveEditor
{
    // [NodeMenuItem("NPBehave/Task/WaitUntilStopped", typeof(NPBehaveGraph))]
    public class NPWaitUntilStoppedNode : ANPTaskNodeBase
    {
        public override string Name => "WaitUntilStopped";

        public NPWaitUntilStoppedData WaitUntilStoppedData = new NPWaitUntilStoppedData() { nodeDes = "WaitUntilStopped" };

        public override ANPNodeDataBase GetNodeData()
        {
            return this.WaitUntilStoppedData;
        }
    }
}