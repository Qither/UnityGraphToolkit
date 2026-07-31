using GraphProcessor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NPBehaveEditor
{
    public class NPBehaveGraphToolbarView : ToolbarView
    {
        private MiniMap m_MiniMap;
        
        public NPBehaveGraphToolbarView(BaseGraphView graphView, MiniMap miniMap) : base(graphView)
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
                    (this.graphView as NPBehaveGraphView)?.AutoSortLayout();
                }, false);
            
            this.AddButton(new GUIContent("Save", "保存行为树"),
                () =>
                { 
                    string assetPath = AssetDatabase.GetAssetPath(this.graphView.graph);
                    string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
                    GUID guid = new GUID(assetGuid);
                    
                    AssetDatabase.SaveAssetIfDirty(guid);
                    AssetDatabase.SaveAssets();
                }, false);
        }
    }
}