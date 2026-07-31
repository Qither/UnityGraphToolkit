using GraphProcessor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace RedDotSystem.Editor
{
    public class RedDotToolbarView : ToolbarView
    {
        private readonly MiniMap m_MiniMap;
        
        public RedDotToolbarView(BaseGraphView graphView, MiniMap miniMap) : base(graphView)
        {
            this.m_MiniMap         = miniMap;
            this.m_MiniMap.visible = false;
        }

        protected override void AddButtons()
        {
            base.AddButtons();

            AddButton("MiniMap", () => { this.m_MiniMap.visible = !this.m_MiniMap.visible; }, left: false);
            
            this.AddButton(new GUIContent("AutoLayout", "自动优化布局"),
                () =>
                {
                    (this.graphView as RedDotGraphView)?.AutoSortLayout();
                }, false);
        }
    }
}