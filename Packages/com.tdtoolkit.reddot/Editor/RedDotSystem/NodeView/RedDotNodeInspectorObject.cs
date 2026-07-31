using GraphProcessor;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RedDotSystem.Editor.NodeView
{
    [CustomEditor(typeof(RedDotNodeInspectorObject))]
    public class RedDotNodeInspectorObjectEditor : NodeInspectorObjectEditor
    {
        private NodeInspectorObject m_NodeInspector;
        protected override void OnEnable()
        {
            this.m_NodeInspector = this.target as NodeInspectorObject;
            
            base.OnEnable();
            
            this.selectedNodeList.styleSheets.Remove(Resources.Load<StyleSheet>("GraphProcessorStyles/InspectorView"));
            this.selectedNodeList.styleSheets.Add(Resources.Load<StyleSheet>("InspectorView"));
        }
        
        protected override void UpdateNodeInspectorList()
        {
            this.selectedNodeList.Clear();

            if (this.m_NodeInspector.selectedNodes.Count == 0)
            {
                this.selectedNodeList.Add(this.placeholder);
                return;
            }

            this.selectedNodeList.Add(this.CreateNodeBlock(this.m_NodeInspector.selectedNodes.First()));
        }
    }
    
    public class RedDotNodeInspectorObject : NodeInspectorObject
    {
    }
}