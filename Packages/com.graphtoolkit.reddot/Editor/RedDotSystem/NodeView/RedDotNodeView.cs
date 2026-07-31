using GraphProcessor;
using RedDotSystem.Editor.Node;
using UnityEngine.UIElements;
using GraphToolkit.Inspector;
using GraphToolkit.Inspector.Editor;
using RedDotSystem.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace RedDotSystem.Editor.NodeView
{
    [NodeCustomEditor(typeof(RedDotNode))]
    public class RedDotNodeView : BaseNodeView
    {
        private RedDotNode m_RedDotNode;
        private bool       m_IsRemove;
        private Button     m_ExecuteButton;

        private readonly List<VisualElement> m_RedDotNodeDataElements = new List<VisualElement>();

        public override void Enable()
        {
            base.Enable();

            this.m_RedDotNode = this.nodeTarget as RedDotNode;
            if (this.m_RedDotNode is null) return;

            this.owner.computeOrderUpdated += this.NodeSetup;
            this.expanded = true;
            this.m_CollapseButton.SetEnabled(false);
            this.m_CollapseButton.SetDisplayed(false);

            this.contentContainer.Q<VisualElement>("top").Q<VisualElement>("divider").style.borderRightWidth = 0;

            this.m_IsRemove = false;
            this.SetNodeColor(this.m_RedDotNode.Data.Number > 0 ? Color.red : Color.clear);
            this.m_RedDotNode.Data.OnNumberChanged += (number) =>
            {
                this.SetNodeColor(number > 0 ? Color.red : Color.clear);
            };
            // this.Highlight();

            this.SetNodeType(this.m_RedDotNode.Data.Node);
            this.SetLinkType(this.m_RedDotNode.Data.Link);
            this.RefreshPortViewParent();

            VisualElement dataFiled = this.controlsContainer;
            VisualElement nodeFiled = this.Q<VisualElement>("RedDotNodeData");

            this.m_RedDotNode.Data.OnNodeTypeChanged += this.SetNodeType;
            this.m_RedDotNode.Data.OnLinkTypeChanged += this.SetLinkType;

            TextField textField = this.Q<VisualElement>("title").Children().First() as TextField;
            textField?.RegisterCallback<FocusOutEvent>(evt =>
            {
                string customName  = textField.value;
                Regex  regex = new Regex(@"^[a-zA-Z]+[a-zA-Z0-9]*");
                Match  match = regex.Match(customName);
                if (match.Success)
                {
                    string matchName = match.Groups[0].Value.Trim();
                    textField.value = matchName.ToUpper();
                    this.m_RedDotNode.SetCustomName(matchName);
                }
                else
                {
                    while (true)
                    {
                        if (string.IsNullOrEmpty(customName) || customName.Length <= 1)
                        {
                            textField.value = RedDotConst.RED_DOT_DEFAULT_NODE_NAME.ToUpper();
                            this.m_RedDotNode.SetCustomName(RedDotConst.RED_DOT_DEFAULT_NODE_NAME.ToUpper());
                            break;
                        }

                        customName = customName.Substring(1, customName.Length - 1);
                        char start      = customName[0];

                        if (!((start > 'A' && start < 'Z') || (start > 'a' && start < 'z'))) continue;

                        match = regex.Match(customName);
                        if (match.Success)
                        {
                            string matchName = match.Groups[0].Value.Trim().ToUpper();
                            textField.value = matchName.ToUpper();
                            this.m_RedDotNode.SetCustomName(matchName);
                        }
                        break;
                    }
                }

                this.m_RedDotNode.Refresh();
                var inputNode = this.m_RedDotNode.GetInputNodes()?.FirstOrDefault() as RedDotNode;
                inputNode?.CheckChildNodeCustomName();
            });

            this.RefreshExecuteFunctionButton();
        }

        public override void Disable()
        {
            if (this.m_RedDotNode is null) return;
            this.owner.computeOrderUpdated -= this.NodeSetup;
        }

        private void NodeSetup()
        {
            this.m_RedDotNode.SetUp();
        }

        private void RefreshExecuteFunctionButton()
        {
            if (this.m_RedDotNode.outputPorts.FirstOrDefault(port => port.fieldName.Equals("WordOutput"))?.GetEdges().Count <= 0 ||
                this.m_RedDotNode.GetOutputNodes().Any(node => node is RedDotMultiNode))
            {
                if (this.contentContainer.Contains(this.m_ExecuteButton)) return;

                this.m_ExecuteButton = new Button(() =>
                {
                    RedDotGraph redDotGraph  = this.owner.graph as RedDotGraph;
                    string      functionName = this.m_RedDotNode.GetFunctionName();
                    if (redDotGraph != null && redDotGraph.RedDotExecuteNodeGenerate.ExecuteNodeCodeInfoMap != null &&
                        redDotGraph.RedDotExecuteNodeGenerate.ExecuteNodeCodeInfoMap.TryGetValue(functionName,
                            out RedDotExecuteNodeCodeInfo codeInfo))
                    {
                        string path = Path.Combine(redDotGraph.ExecuteNodePath, codeInfo.FileName + ".cs");
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, codeInfo.StartLine);
                    }
                }) { text = "ExecuteFunction", };
                this.controlsContainer.Add(this.m_ExecuteButton);
            }
            else
            {
                if (this.m_ExecuteButton != null)
                {
                    this.controlsContainer.Remove(this.m_ExecuteButton);
                    this.m_ExecuteButton = null;
                }
            }
        }

        private void SetNodeType(RedDotNode.NodeType nodeType)
        {
            PortView inputPortView = this.inputPortViews.First(x => x.portData.displayName == "WordInput");
            if ((nodeType & RedDotNode.NodeType.Input) != 0)
            {
                inputPortView.SetDisplayed(true);
                inputPortView.SetEnabled(true);
                PortData portData = inputPortView.portData;
                portData.displayType = typeof(string);
                inputPortView.UpdatePortView(portData);
            }
            else
            {
                inputPortView.SetDisplayed(false);
                inputPortView.SetEnabled(false);
                PortData portData = inputPortView.portData;
                portData.displayType = typeof(RedDotNode.RedDotNodeData);
                inputPortView.UpdatePortView(portData);

                EdgeView[] edgeViews = inputPortView.GetEdges().ToArray();
                for (int i = edgeViews.Length; i > 0; i--)
                {
                    EdgeView edge = edgeViews[i - 1];
                    this.owner.Disconnect(edge);
                }
            }

            PortView outputPortView = this.outputPortViews.First(x => x.portData.displayName == "WordOutput");
            if ((nodeType & RedDotNode.NodeType.Output) != 0)
            {
                outputPortView.SetDisplayed(true);
                outputPortView.SetEnabled(true);
                PortData portData = outputPortView.portData;
                portData.displayType = typeof(string);
                outputPortView.UpdatePortView(portData);
            }
            else
            {
                outputPortView.SetDisplayed(false);
                outputPortView.SetEnabled(false);
                PortData portData = outputPortView.portData;
                portData.displayType = typeof(RedDotNode.RedDotNodeData);
                outputPortView.UpdatePortView(portData);

                EdgeView[] edgeViews = outputPortView.GetEdges().ToArray();
                for (int i = edgeViews.Length; i > 0; i--)
                {
                    EdgeView edge = edgeViews[i - 1];
                    this.owner.Disconnect(edge);
                }
            }

            this.RefreshPortViewParent();
        }

        private void SetLinkType(RedDotNode.LinkType linkType)
        {
            PortView inputPortView = this.inputPortViews.First(x => x.portData.displayName == "LinkInput");
            if ((linkType & RedDotNode.LinkType.LinkInput) != 0)
            {
                inputPortView.SetDisplayed(true);
                inputPortView.SetEnabled(true);
                PortData portData = inputPortView.portData;
                portData.displayType = typeof(RedDotNode);
                inputPortView.UpdatePortView(portData);
            }
            else
            {
                inputPortView.SetDisplayed(false);
                inputPortView.SetEnabled(false);
                PortData portData = inputPortView.portData;
                portData.displayType = typeof(RedDotNode.RedDotNodeData);
                inputPortView.UpdatePortView(portData);
                EdgeView[] edgeViews = inputPortView.GetEdges().ToArray();
                for (int i = edgeViews.Length; i > 0; i--)
                {
                    EdgeView edge = edgeViews[i - 1];
                    this.owner.Disconnect(edge);
                }
            }

            PortView outputPortView = this.outputPortViews.First(x => x.portData.displayName == "LinkOutput");
            if ((linkType & RedDotNode.LinkType.LinkOutput) != 0)
            {
                outputPortView.SetDisplayed(true);
                outputPortView.SetEnabled(true);
                PortData portData = outputPortView.portData;
                portData.displayType = typeof(RedDotNode);
                outputPortView.UpdatePortView(portData);
            }
            else
            {
                outputPortView.SetDisplayed(false);
                outputPortView.SetEnabled(false);
                PortData portData = outputPortView.portData;
                portData.displayType = typeof(RedDotNode.RedDotNodeData);
                outputPortView.UpdatePortView(portData);
                EdgeView[] edgeViews = outputPortView.GetEdges().ToArray();
                for (int i = edgeViews.Length; i > 0; i--)
                {
                    EdgeView edge = edgeViews[i - 1];
                    this.owner.Disconnect(edge);
                }
            }

            this.RefreshPortViewParent();
        }

        private void RefreshLinkOutput()
        {
            RedDotNode.LinkType linkType = this.m_RedDotNode.Data.Link;
            PortView outputPortView = this.outputPortViews.First(x => x.portData.displayName == "LinkOutput");
            if ((linkType & RedDotNode.LinkType.LinkOutput) != 0)
            {
                RuleData ruleData = this.m_RedDotNode.Rule.Rules.FirstOrDefault(x => x.Name.Equals(nameof(RedDotLinkRule)));
                if (ruleData != null)
                {
                    this.m_RedDotNode.Rule.Rules.Remove(ruleData);
                    this.m_RedDotNode.Rule.Rules.Insert(0, ruleData);
                    this.owner.UpdateNodeInspectorSelection();
                }

                if (outputPortView?.connectionCount > 0 && ruleData == null)
                {
                    this.m_RedDotNode.Rule.Rules.Insert(0, this.m_RedDotNode.Rule.CreateItem() as RuleData);
                    this.m_RedDotNode.Rule.Rules[0].Name = nameof(RedDotLinkRule);
                }

                if (outputPortView?.connectionCount <= 0 && ruleData != null)
                {
                    this.m_RedDotNode.Rule.Rules.Remove(ruleData);
                }
            }
            else
            {
                RuleData ruleData = this.m_RedDotNode.Rule.Rules.FirstOrDefault(x => x.Name.Equals(nameof(RedDotLinkRule)));
                if (ruleData != null)
                {
                    this.m_RedDotNode.Rule.Rules.Remove(ruleData);
                }
            }
        }

        private void RefreshPortViewParent()
        {
            if ((this.m_RedDotNode.Data.Node & RedDotNode.NodeType.Input) == 0 &&
                (this.m_RedDotNode.Data.Link & RedDotNode.LinkType.LinkInput) == 0)
            {
                PortView inputPortView = this.inputPortViews.First(x => x.portData.displayName == "WordInput");
                inputPortView.parent.SetDisplayed(false);
            }
            else
            {
                PortView inputPortView = this.inputPortViews.First(x => x.portData.displayName == "WordInput");
                inputPortView.parent.SetDisplayed(true);
            }

            if ((this.m_RedDotNode.Data.Node & RedDotNode.NodeType.Output) == 0 &&
                (this.m_RedDotNode.Data.Link & RedDotNode.LinkType.LinkOutput) == 0)
            {
                PortView outputPortView = this.outputPortViews.First(x => x.portData.displayName == "WordOutput");
                outputPortView.parent.SetDisplayed(false);
            }
            else
            {
                PortView outputPortView = this.outputPortViews.First(x => x.portData.displayName == "WordOutput");
                outputPortView.parent.SetDisplayed(true);
            }
        }

        public override bool RefreshPorts()
        {
            if (!this.m_IsRemove)
            {
                this.m_RedDotNode.CheckChildNodeCustomName();
                this.m_RedDotNode?.Refresh();
                this.RefreshExecuteFunctionButton();
                this.RefreshLinkOutput();
            }

            return true;
        }

        public override void OnRemoved()
        {
            this.m_IsRemove = true;
        }
    }
}