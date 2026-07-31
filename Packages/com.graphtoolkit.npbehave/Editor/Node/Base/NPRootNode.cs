using GraphProcessor;
using NPBehave;

namespace NPBehaveEditor
{
    [System.Serializable]
    public class NPRootNode : ANPNodeBase
    {
        
        public override string      name     => "Root";
        public override string      Icon     => "DarkEntryIcon";

        [Output("NextNode", false), Vertical]
        public ANPNodeBase NextNode;
        
        [ShowInInspector]
        public NPRootNodeData RootNodeData = new NPRootNodeData();

        public override ANPNodeDataBase GetNodeData()
        {
            return this.RootNodeData;
        }
    }
}
