using System.Collections.Generic;
using System.Linq;

namespace NPBehave
{
    public abstract class Composite : Container
    {
        protected readonly Node[] Children;

        public Composite(string name, Node[] children) : base(name)
        {
            this.Children = children;
            Log.AssertIsTrue(children.Length > 0, "Composite nodes (Selector, Sequence, Parallel) need at least one child!");

            foreach (Node node in this.Children)
            {
                node.SetParent(this);
            }
        }

        public override void SetRoot(Root rootNode)
        {
            base.SetRoot(rootNode);

            foreach (Node node in this.Children)
            {
                node.SetRoot(rootNode);
            }
        }


#if UNITY_EDITOR
        public override IEnumerable<Node> DebugChildren => this.Children;

        public Node DebugGetActiveChild()
        {
            return this.DebugChildren.FirstOrDefault(node => node.CurrentState == Node.State.Active);
        }
#endif

        protected override void Stopped(bool success)
        {
            foreach (Node child in this.Children)
            {
                child.ParentCompositeStopped(this);
            }
            base.Stopped(success);
        }

        public abstract void StopLowerPriorityChildrenForChild(Node child, bool immediateRestart);
    }
}