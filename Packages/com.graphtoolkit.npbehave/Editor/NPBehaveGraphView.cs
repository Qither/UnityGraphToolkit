using GraphNodeLayoutExpansion.Runtime;
using GraphProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Group = UnityEditor.Experimental.GraphView.Group;
using StickyNote = UnityEditor.Experimental.GraphView.StickyNote;

namespace NPBehaveEditor
{
    public class NPBehaveGraphView : BaseGraphView
    {
        public static Dictionary<string, NPRootNode> RootNodes = new Dictionary<string, NPRootNode>();
        
        public static void AddRootNode(string path, NPRootNode rootNode)
        {
            if (RootNodes.ContainsKey(path))
            {
                RootNodes[path] = rootNode;
            }
            else
            {
                RootNodes.Add(path, rootNode);
            }
        }
        
        public static NPRootNode GetRootNode(string path)
        {
            if (RootNodes.ContainsKey(path))
            {
                return RootNodes[path];
            }
            return null;
        }

        private readonly NPBehaveGraphNodeMenuWindow m_NpBehaveGraphNodeMenu;
        public NPBehaveGraphView(EditorWindow window) : base(window)
        {
            this.m_NpBehaveGraphNodeMenu = ScriptableObject.CreateInstance<NPBehaveGraphNodeMenuWindow>();
            this.m_NpBehaveGraphNodeMenu.Initialize(this, window);
        }

        protected override void InitializeView()
        {
            this.nodeCreationRequest = (c) => SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), this.m_NpBehaveGraphNodeMenu);
            if (this.graph is NPBehaveGraph behaveGraph)
            {
                if (behaveGraph.nodes.Count == 0)
                {
                    AddNode(BaseNode.CreateFromType(typeof(NPRootNode), Vector2.zero));
                }
            }
        }

        protected override BaseEdgeConnectorListener CreateEdgeConnectorListener() => new NPNodeViewEdgeConnectorListener(this);

        
        protected override bool canDeleteSelection => this.selection.Cast<GraphElement>().Any<GraphElement>((Func<GraphElement, bool>) (e =>
        {
            switch (e)
            {
                case null:
                case NPNodeView { nodeTarget: NPRootNode _ }:
                    return false;
                default:
                    return (e.capabilities & Capabilities.Deletable) != 0;
            }
        }));

        protected override bool canCopySelection => this.selection.Any<ISelectable>((Func<ISelectable, bool>) (s =>
        {
            NPNodeView nodeView = s as NPNodeView;
            switch (s)
            {
                case Node _:
                    return !(nodeView is { nodeTarget: NPRootNode _ });
                case Group _:
                case Placemat _:
                    return true;
                default:
                    return s is StickyNote;
            }
        }));

        protected override bool canCutSelection => this.canCopySelection;
        
        protected override NodeInspectorObject CreateNodeInspectorObject()
        {
            var inspector = ScriptableObject.CreateInstance<NPBehaveNodeInspectorObject>();
            inspector.name      = "Node Inspector";
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
            BaseNodeView root = this.nodeViews.First();
            NodeAutoLayoutBuilder.Layout(new NPBehaveNodeConvertor().Init(root, "NextNode"));
        }
        
        void CreateNodeOfType(Type type, Vector2 position)
        {
            RegisterCompleteObjectUndo("Added " + type + " node");
            AddNode(BaseNode.CreateFromType(type, position));
        }
    }
}
