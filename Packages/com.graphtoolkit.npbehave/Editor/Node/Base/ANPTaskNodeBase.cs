using GraphProcessor;

namespace NPBehaveEditor
{
    public abstract class ANPTaskNodeBase : ANPNodeBase
    {
        [Input("PreNode"), Vertical]
        public ANPNodeBase PrevNode;

        public override string     Icon     => "DarkActionIcon";
    }
}