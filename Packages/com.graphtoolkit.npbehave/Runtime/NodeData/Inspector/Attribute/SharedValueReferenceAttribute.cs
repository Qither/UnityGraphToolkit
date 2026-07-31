#if UNITY_EDITOR
using UnityEngine;

namespace NPBehave
{
    public class SharedValueReferenceAttribute : PropertyAttribute
    {
        public string CustomLabel { get; private set; }
        
        public SharedValueReferenceAttribute(string label = null)
        {
            this.CustomLabel = label;
        }
    }
}
#endif