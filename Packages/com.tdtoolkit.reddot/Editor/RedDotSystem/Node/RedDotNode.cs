using GraphProcessor;
using PiRhoSoft.Utilities;
using RedDotSystem.Runtime;
using SerializeExpansion.Runtime;
using System;
using System.Collections.Generic;
using UnityEditor.UIElements;

namespace RedDotSystem.Editor.Node
{
    [Serializable]
    [NodeMenuItem("RedDot/RedDotNode", typeof(RedDotGraph))]
    public class RedDotNode : BaseNode
    {
        [Flags]
        public enum NodeType
        {
            Input = 1 << 0,
            Output = 1 << 1
        }

        [Flags]
        public enum LinkType
        {
            LinkInput = 1 << 0,
            LinkOutput = 1 << 1
        }

        [Serializable]
        public class RedDotNodeData
        {
            public NodeType Node = NodeType.Input;

            public LinkType Link;

            public string Desc;

            public int Order;

            public string Key;

            public int Number;

            [NonSerialized]
            public RedDotGraph Graph;

            public PopupValues<int> GetSystem => this.Graph?.Settings?.GetSystemValues() ??
                new PopupValues<int>
                {
                    Values = new List<int> { 0 },
                    Options = new List<string> { "Always available" }
                };

            [Popup(nameof(GetSystem))]
            [CustomLabel("System")]
            public int System;

            public event Action<NodeType> OnNodeTypeChanged;

            public event Action<LinkType> OnLinkTypeChanged;

            public event Action<int> OnNumberChanged;

            public void NodeChanged(SerializedPropertyChangeEvent changeEvent)
            {
                this.OnNodeTypeChanged?.Invoke(this.Node);
            }

            public void LinkChanged(SerializedPropertyChangeEvent changeEvent)
            {
                this.OnLinkTypeChanged?.Invoke(this.Link);
            }

            public void NumberChanged(SerializedPropertyChangeEvent changeEvent)
            {
                this.OnNumberChanged?.Invoke(this.Number);
            }
        }

        [Input("WordInput")]
        public string WordInput;

        [Output("WordOutput")]
        public string WordOutput;

        [Input("LinkInput")]
        public RedDotNode LinkInput;

        [Output("LinkOutput")]
        public RedDotNode LinkOutput;

        [ShowInInspector]
        public RedDotNodeData Data = new RedDotNodeData();

        public override bool isRenamable => true;

        [ShowInInspector]
        public NodeRule Rule = new NodeRule();

        public void SetUp()
        {
            this.Data.Order = this.computeOrder;
            this.Data.Graph = this.graph as RedDotGraph;
            this.Rule.Graph = this.graph as RedDotGraph;
            foreach (RuleData ruleData in this.Rule.Rules)
            {
                ruleData.Graph = this.graph as RedDotGraph;
                ruleData.NodeRule = this.Rule;
            }
        }
    }
}
