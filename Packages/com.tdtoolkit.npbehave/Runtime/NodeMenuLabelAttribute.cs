using System;

namespace NPBehave
{
    public class NodeMenuLabelAttribute : Attribute
    {
        public string Label { get; private set; }
        
        public NodeMenuLabelAttribute(string label)
        {
            this.Label = label;
        }
    }
}