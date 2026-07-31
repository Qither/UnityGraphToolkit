using GraphProcessor;

namespace NPBehaveEditor
{
    public abstract class ANPDecoratorNodeBase : ANPNodeBase
    {
        [Input("PreNode"), Vertical]
        public ANPNodeBase PrevNode;
        
        [Output("NextNode", false), Vertical]
        public ANPNodeBase NextNode;
    }
}