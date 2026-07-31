using GraphProcessor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NPBehaveEditor
{
    public class NPNodeViewEdgeConnectorListener : BaseEdgeConnectorListener
    {
        private NPBehaveGraphNodeMenuWindow m_EdgeNodeNpBehaveGraphMenuWindow;
        public NPNodeViewEdgeConnectorListener(BaseGraphView graphView) : base(graphView)
        {
        }

        public override void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            this.graphView.RegisterCompleteObjectUndo("Disconnect edge");

            //If the edge was already existing, remove it
            if (!edge.isGhostEdge)
                graphView.Disconnect(edge as EdgeView);

            // when on of the port is null, then the edge was created and dropped outside of a port
            if (edge.input == null || edge.output == null)
                ShowNodeCreationMenuFromEdge(edge as EdgeView, position);
        }
        
        void ShowNodeCreationMenuFromEdge(EdgeView edgeView, Vector2 position)
        {
            if (this.m_EdgeNodeNpBehaveGraphMenuWindow == null)
                this.m_EdgeNodeNpBehaveGraphMenuWindow = ScriptableObject.CreateInstance<NPBehaveGraphNodeMenuWindow>();

            this.m_EdgeNodeNpBehaveGraphMenuWindow.Initialize(graphView, EditorWindow.focusedWindow, edgeView);
            SearchWindow.Open(new SearchWindowContext(position + EditorWindow.focusedWindow.position.position), this.m_EdgeNodeNpBehaveGraphMenuWindow);
        }
    }
}