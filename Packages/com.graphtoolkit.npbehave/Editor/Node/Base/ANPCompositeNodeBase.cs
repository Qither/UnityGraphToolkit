using GraphProcessor;

namespace NPBehaveEditor
{
    public abstract class ANPCompositeNodeBase : ANPNodeBase
    {
        [Input("PreNode"), Vertical]
        public ANPNodeBase PrevNode;

        [Output("NextNode"), Vertical]
        public ANPNodeBase NextNode;
    }
}