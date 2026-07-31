﻿using GraphNodeLayoutExpansion.Editor;
using GraphNodeLayoutExpansion.Runtime;
using GraphProcessor;
using RedDotSystem.Editor.NodeView;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using StickyNote = GraphProcessor.StickyNote;

namespace RedDotSystem.Editor
{
    public class RedDotGraphView : BaseGraphView
    {
        private readonly RedDotGraphNodeMenuWindow m_NodeCreateMenuWindow;
        private          Action                    m_BindEvent;
        private          Action                    m_UnBindEvent;
        private readonly RedDotGraphWindow         m_Window;

        private       int m_Count;
        private const int m_Interval = 500;
        private       int m_PreCount;
        private       int m_CurTime;

        private bool m_IsBind;

        public RedDotGraphView(EditorWindow window) : base(window)
        {
            this.m_Window = window as RedDotGraphWindow;
            this.m_NodeCreateMenuWindow = ScriptableObject.CreateInstance<RedDotGraphNodeMenuWindow>();
            this.m_NodeCreateMenuWindow.Initialize(this, window);
        }


        protected override void InitializeView()
        {
            this.nodeCreationRequest = (c) =>
                SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), this.m_NodeCreateMenuWindow);

            if (this.stickyNoteViews != null && this.stickyNoteViews.Count > 0)
            {
                foreach (StickyNoteView stickyNoteView in this.stickyNoteViews)
                {
                    stickyNoteView.fontSize = StickyNoteFontSize.Large;
                }
            }
        }

        protected override void BuildStickyNoteContextualMenu(ContextualMenuPopulateEvent evt, int menuPosition = -1)
        {
            if (menuPosition == -1)
                menuPosition = evt.menu.MenuItems().Count;
#if UNITY_2020_1_OR_NEWER
            Vector2 position = (evt.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            evt.menu.InsertAction(menuPosition, "Create Sticky Note", (e) => AddLargeStickyNote(new StickyNote("Create Note", position)), DropdownMenuAction.AlwaysEnabled);
#endif
        }

#if UNITY_2020_1_OR_NEWER
        private StickyNoteView AddLargeStickyNote(StickyNote note)
        {
            StickyNoteView view = base.AddStickyNote(note);
            view.fontSize = StickyNoteFontSize.Large;
            return view;
        }
#endif

        protected override NodeInspectorObject CreateNodeInspectorObject()
        {
            var inspector = ScriptableObject.CreateInstance<RedDotNodeInspectorObject>();
            inspector.name      = "RedDot Inspector";
            inspector.hideFlags = HideFlags.HideAndDontSave ^ HideFlags.NotEditable;

            return inspector;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendSeparator();

            // Sort alphabetically using the upstream NodeGraphProcessor menu contract.
            var sortedMenuItems = NodeProvider.GetNodeMenuEntries(this.graph)
                .OrderBy(item => item.path);

            foreach (var nodeMenuItem in sortedMenuItems)
            {
                var     mousePos     = (evt.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
                Vector2 nodePosition = mousePos;
                evt.menu.AppendAction(nodeMenuItem.path,
                    (e) => CreateNodeOfType(nodeMenuItem.type, nodePosition),
                    DropdownMenuAction.AlwaysEnabled
                );
            }

            base.BuildContextualMenu(evt);
        }

        public void AutoSortLayout()
        {
            if (!(this.selection.FirstOrDefault() is BaseNodeView selectedNodeView))
            {
                return;
            }

            NodeAutoLayoutBuilder.Layout(new GraphNodeConvertor().Init(selectedNodeView, "WordOutput"));
        }

        void CreateNodeOfType(Type type, Vector2 position)
        {
            RegisterCompleteObjectUndo("Added " + type + " node");
            AddNode(BaseNode.CreateFromType(type, position));
        }
    }
}
