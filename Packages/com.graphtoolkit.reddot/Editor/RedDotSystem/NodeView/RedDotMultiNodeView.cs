using GraphProcessor;
using PiRhoSoft.Utilities.Editor;
using RedDotSystem.Editor.Node;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RedDotSystem.Editor.NodeView
{
    [NodeCustomEditor(typeof(RedDotMultiNode))]
    public class RedDotMultiNodeView : BaseNodeView
    {
        private RedDotMultiNode m_RedDotMultiNode;

        private readonly List<VisualElement> m_RedDotNodeDataElements = new List<VisualElement>();

        public override void Enable()
        {
            base.Enable();
            this.m_RedDotMultiNode = this.nodeTarget as RedDotMultiNode;

            if (this.m_RedDotMultiNode is null) return;

            this.expanded = true;
            this.m_CollapseButton.SetEnabled(false);
            this.m_CollapseButton.SetDisplayed(false);

            this.m_RedDotMultiNode.SetUp();
            this.contentContainer.Q<VisualElement>("top").Q<VisualElement>("divider").style.borderRightWidth = 0;
        }
    }
}