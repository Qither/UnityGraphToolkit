using GraphProcessor;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NPBehaveEditor
{
    public class NPBehaveGraphWindow : BaseGraphWindow
    {
        protected MiniMap MiniMap;
        protected NPBehaveGraphToolbarView ToolbarView;
        protected override void OnDisable()
        {
            base.OnDisable();

            if (this.graph != null && this.graphView != null)
            {
                GraphWindowHelper.RemoveGraphWindow(this.graph);
            }
        }

        protected override void InitializeWindow(BaseGraph graph)
        {
            titleContent = new GUIContent($"Behave {graph.name}");
            if (this.graphView == null)
            {
                GridBackground grid = new GridBackground();
                grid.StretchToParentWidth();

                this.graphView   = new NPBehaveGraphView(this);
                this.graphView.Insert(0, grid);
                StyleSheet styleSheet = Resources.Load<StyleSheet>("NPBehaveGraphStyles/GraphView");
                this.graphView.styleSheets.Add(styleSheet);
                
                this.MiniMap     = new MiniMap() { anchored = true };
                this.ToolbarView = new NPBehaveGraphToolbarView(this.graphView, this.MiniMap);
                this.graphView.Add(this.MiniMap);
                this.graphView.Add(this.ToolbarView);
            }
            this.rootView.Add(this.graphView);
        }

        private void OnGUI()
        {
            MiniMap?.SetPosition(new Rect(this.position.size.x - 205, this.position.size.y - 205, 200, 200));
        }
    }
}