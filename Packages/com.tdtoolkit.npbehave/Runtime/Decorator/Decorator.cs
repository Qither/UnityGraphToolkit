using System.Collections.Generic;

namespace NPBehave
{

    public abstract class Decorator : Container
    {
        protected readonly Node Decorate;

        public Decorator(string name, Node decorate) : base(name)
        {
            this.Decorate = decorate;
            this.Decorate.SetParent(this);
        }

        public override void SetRoot(Root rootNode)
        {
            base.SetRoot(rootNode);
            this.Decorate.SetRoot(rootNode);
        }


#if UNITY_EDITOR
        public override IEnumerable<Node> DebugChildren
        {
            get
            {
                return new Node[] { this.Decorate };
            }
        }
#endif

        public override void ParentCompositeStopped(Composite composite)
        {
            base.ParentCompositeStopped(composite);
            this.Decorate.ParentCompositeStopped(composite);
        }
    }
}