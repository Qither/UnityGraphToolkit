using GraphProcessor;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RedDotSystem.Editor
{
    public class RedDotGraphWindow : BaseGraphWindow
    {
        private MiniMap m_MiniMap;

        private ToolbarView m_ToolbarView;

        public RedDotGraph RedDotGraph => this.graph as RedDotGraph;

        [NonSerialized]
        public Action OnUpdate;

        protected override void InitializeWindow(BaseGraph baseGraph)
        {
            this.titleContent = new GUIContent(baseGraph.name);
            if (this.graphView == null)
            {
                GridBackground grid = new GridBackground();
                grid.StretchToParentWidth();

                this.graphView   = new RedDotGraphView(this);
                this.graphView.Insert(0, grid);
                StyleSheet styleSheet = Resources.Load<StyleSheet>("GraphView");
                if (styleSheet != null)
                {
                    this.graphView.styleSheets.Add(styleSheet);
                }
                this.m_MiniMap     = new MiniMap() { anchored = true };
                this.graphView.Add(this.m_MiniMap);
                this.m_ToolbarView = new RedDotToolbarView(this.graphView, this.m_MiniMap);
                this.graphView.Add(this.m_ToolbarView);
                this.rootView.Add(this.graphView);
            }
        }

        private void OnGUI()
        {
            this.m_MiniMap?.SetPosition(new Rect(this.position.size.x - 205, this.position.size.y - 205, 200, 200));
        }

        protected override void Update()
        {
            // base.Update();

            this.OnUpdate?.Invoke();
        }
    }
}
