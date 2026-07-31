using System.Collections.Generic;

namespace NPBehave
{
    public abstract class Container : Node
    {
        public bool Collapse { get; set; }

        public Container(string name) : base(name)
        {
        }

        public void ChildStopped(Node child, bool succeeded)
        {
            Log.AssertAreNotEqual(this.NodeState, State.Inactive, "A Child of a Container was stopped while the container was inactive.");
            this.DoChildStopped(child, succeeded);
        }

        protected abstract void DoChildStopped(Node child, bool succeeded);

#if UNITY_EDITOR
        public abstract IEnumerable<Node> DebugChildren
        {
            get;
        }
#endif
    }
}