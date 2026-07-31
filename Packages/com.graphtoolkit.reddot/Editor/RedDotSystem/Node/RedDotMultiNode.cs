using GraphProcessor;
using GraphToolkit.Inspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedDotSystem.Editor.Node
{
    [Serializable]
    [NodeMenuItem("RedDot/RedDotMultiNode", typeof(RedDotGraph))]
    public class RedDotMultiNode : BaseNode
    {
        [Input("WordInput")]
        public string WordInput;

        [ShowInInspector]
        public RedDotMultiNodeData Data = new RedDotMultiNodeData();

        [ShowInInspector]
        public NodeRule Rule = new NodeRule();

        public void SetUp()
        {
            this.Data.Graph = this.graph as RedDotGraph;
            this.Rule.Graph = this.graph as RedDotGraph;
            foreach (RuleData ruleData in this.Rule.Rules)
            {
                ruleData.Graph = this.graph as RedDotGraph;
                ruleData.NodeRule = this.Rule;
            }
        }

        [Serializable]
        public class RedDotMultiNodeData
        {
            [Popup(nameof(GetRedDotMultiData), false)]
            public string MultiNodeData = "None";

            public PopupValues<int> GetSystem => (this.Graph as RedDotGraph)?.Settings?.GetSystemValues() ??
                new PopupValues<int>
                {
                    Values = new List<int> { 0 },
                    Options = new List<string> { "Always available" }
                };

            [Popup(nameof(GetSystem))]
            [CustomLabel("System")]
            public int System;

            [NonSerialized]
            public BaseGraph Graph;

            private List<string> GetRedDotMultiData()
            {
                if (this.Graph is RedDotGraph && RedDotHelper.RedDotMultiNodeDataTypes.Count > 0)
                {
                    List<string> multiData = RedDotHelper.RedDotMultiNodeDataTypes.Keys
                        .OrderBy(name => name, StringComparer.Ordinal)
                        .ToList();
                    multiData.Insert(0, "None");
                    return multiData;
                }

                return new List<string> { "None" };
            }
        }
    }
}
