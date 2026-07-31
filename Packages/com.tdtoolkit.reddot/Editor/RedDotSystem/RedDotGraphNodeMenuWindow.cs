﻿using GraphProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RedDotSystem.Editor
{
    public class RedDotGraphNodeMenuWindow : ScriptableObject, ISearchWindowProvider
    {
        private BaseGraphView m_GraphView;
        private EditorWindow  m_Window;
        private Texture2D     m_Icon;
        private EdgeView      m_EdgeFilter;
        private PortView      m_InputPortView;
        private PortView      m_OutputPortView;

        public void Initialize(BaseGraphView graphView, EditorWindow window, EdgeView edgeFilter = null)
        {
            this.m_GraphView      = graphView;
            this.m_Window         = window;
            this.m_EdgeFilter     = edgeFilter;
            this.m_InputPortView  = edgeFilter?.input as PortView;
            this.m_OutputPortView = edgeFilter?.output as PortView;

            // Transparent icon to trick search window into indenting items
            if (this.m_Icon == null)
                this.m_Icon = new Texture2D(1, 1);
            this.m_Icon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            this.m_Icon.Apply();
        }

        void OnDestroy()
        {
            if (this.m_Icon != null)
            {
                DestroyImmediate(this.m_Icon);
                this.m_Icon = null;
            }
        }
        
         public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry> { new SearchTreeGroupEntry(new GUIContent("NPBehaveNode"), 0), };

            if (this.m_EdgeFilter == null)
                CreateStandardNodeMenu(tree);
            else
                CreateEdgeNodeMenu(tree);

            return tree;
        }

        void CreateStandardNodeMenu(List<SearchTreeEntry> tree)
        {
            // Sort menu by order first, then alphabetical order and submenus
            var nodeEntries = this.m_GraphView.FilterCreateNodeMenuEntries().OrderBy(k => k.order).ThenBy(k => k.path);
            var titlePaths  = new HashSet<string>();

            foreach (var nodeMenuItem in nodeEntries)
            {
                var nodePath = nodeMenuItem.path;
                var nodeName = nodePath;
                var level    = 0;
                var parts    = nodePath.Split('/');

                if (parts.Length > 1)
                {
                    level++;
                    nodeName = parts[parts.Length - 1];
                    var fullTitleAsPath = "";

                    for (var i = 0; i < parts.Length - 1; i++)
                    {
                        var title = parts[i];
                        fullTitleAsPath += title;
                        level           =  i + 1;

                        // Add section title if the node is in subcategory
                        if (!titlePaths.Contains(fullTitleAsPath))
                        {
                            tree.Add(new SearchTreeGroupEntry(new GUIContent(title)) { level = level });
                            titlePaths.Add(fullTitleAsPath);
                        }
                    }
                }

                tree.Add(new SearchTreeEntry(new GUIContent(nodeName, this.m_Icon)) { level = level + 1, userData = nodeMenuItem.type });
            }
        }

        void CreateEdgeNodeMenu(List<SearchTreeEntry> tree)
        {
            var entries = NodeProvider.GetEdgeCreationNodeMenuEntry((this.m_EdgeFilter.input ?? this.m_EdgeFilter.output) as PortView, this.m_GraphView.graph);

            var titlePaths = new HashSet<string>();

            var nodePaths = NodeProvider.GetNodeMenuEntries(this.m_GraphView.graph);

            /*
            tree.Add(new SearchTreeEntry(new GUIContent($"Relay", icon))
            {
                level = 1,
                userData = new NodeProvider.PortDescription
                {
                    nodeType        = typeof(RelayNode),
                    portType        = typeof(System.Object),
                    isInput         = inputPortView != null,
                    portFieldName   = inputPortView != null ? nameof(RelayNode.output) : nameof(RelayNode.input),
                    portIdentifier  = "0",
                    portDisplayName = inputPortView != null ? "Out" : "In",
                }
            });
            */
            
            var sortedMenuItems = entries.Select(port => {
                var nodeInfo = nodePaths.FirstOrDefault(kp => kp.type == port.nodeType);
                return (port, nodeInfo.path, nodeInfo.order);
            }).OrderBy(e => e.order).ThenBy(e => e.path);

            // Sort menu by alphabetical order and submenus
            foreach (var nodeMenuItem in sortedMenuItems)
            {
                var nodePath = nodeMenuItem.path;

                // Ignore the node if it's not in the create menu
                if (String.IsNullOrEmpty(nodePath))
                    continue;

                var nodeName = nodePath;
                var level    = 0;
                var parts    = nodePath.Split('/');

                if (parts.Length > 1)
                {
                    level++;
                    nodeName = parts[parts.Length - 1];
                    var fullTitleAsPath = "";

                    for (var i = 0; i < parts.Length - 1; i++)
                    {
                        var title = parts[i];
                        fullTitleAsPath += title;
                        level           =  i + 1;

                        // Add section title if the node is in subcategory
                        if (!titlePaths.Contains(fullTitleAsPath))
                        {
                            tree.Add(new SearchTreeGroupEntry(new GUIContent(title)) { level = level });
                            titlePaths.Add(fullTitleAsPath);
                        }
                    }
                }
                
                tree.Add(new SearchTreeEntry(new GUIContent($"{nodeName}:  {nodeMenuItem.port.portDisplayName}", this.m_Icon))
                {
                    level = level + 1, userData = nodeMenuItem.port
                });
                
            }
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            // window to graph position
            var windowRoot          = this.m_Window.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - m_Window.position.position);
            var graphMousePosition  = this.m_GraphView.contentViewContainer.WorldToLocal(windowMousePosition);

            var nodeType = searchTreeEntry.userData is Type
                ? (Type)searchTreeEntry.userData
                : ((NodeProvider.PortDescription)searchTreeEntry.userData).nodeType;

            this.m_GraphView.RegisterCompleteObjectUndo("Added " + nodeType);
            var view = this.m_GraphView.AddNode(BaseNode.CreateFromType(nodeType, graphMousePosition));

            if (searchTreeEntry.userData is NodeProvider.PortDescription desc)
            {
                var targetPort = view.GetPortViewFromFieldName(desc.portFieldName, desc.portIdentifier);
                if (this.m_InputPortView == null)
                    m_GraphView.Connect(targetPort, m_OutputPortView);
                else
                    m_GraphView.Connect(m_InputPortView, targetPort);
            }

            return true;
        }
    }
}