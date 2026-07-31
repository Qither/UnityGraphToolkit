namespace GraphToolkit.Inspector
{
    public class CustomSerializeListAttribute : PropertyTraitAttribute
    {
        public const string ALWAYS = "";
        public const string NEVER  = null;

        public string AllowAdd    = ALWAYS;
        public string AllowRemove = ALWAYS;

        public bool AllowReorder  = true;
        public bool IsCollapsable = true;
        
        public bool AllowDerived  = true;

        public string EmptyLabel = null;
        
        public string CustomLabel = null;

        public string AddItem         = null;
        public string AddCallback     = null;
        public string RemoveCallback  = null;
        public string ReorderCallback = null;
        public string ChangeCallback  = null;

        public CustomSerializeListAttribute() : base(ContainerPhase, 0)
        {
        }
    }
}