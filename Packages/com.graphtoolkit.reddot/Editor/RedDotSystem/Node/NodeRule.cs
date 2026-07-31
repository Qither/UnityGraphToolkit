using GraphProcessor;
using GraphToolkit.Inspector;
using SerializeExpansion.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedDotSystem.Editor.Node
{
    [Serializable]
    public class NodeRule
    {
        [SerializeList(AddItem = nameof(CreateItem))]
        public RuleList Rules = new RuleList();

        [NonSerialized]
        public BaseGraph Graph;

        public object CreateItem()
        {
            return new RuleData { Graph = this.Graph, NodeRule = this };
        }
    }

    [Serializable]
    public class RuleList : SerializeReferenceList<RuleData>
    {
    }

    [Serializable]
    public class RuleData
    {
        [NoLabel]
        [Popup(nameof(GetRuleNames))]
        public string Name = "None";

        [NonSerialized]
        public BaseGraph Graph;

        [NonSerialized]
        public NodeRule NodeRule;

        private List<string> GetRuleNames()
        {
            if (this.Graph is not RedDotGraph || RedDotHelper.RedDotRuleTypes.Count == 0)
            {
                return new List<string> { "None" };
            }

            List<string> ruleTypes = RedDotHelper.RedDotRuleTypes.Keys
                .Where(ruleName => !this.ContainsRule(ruleName))
                .OrderBy(ruleName => ruleName, StringComparer.Ordinal)
                .ToList();
            ruleTypes.Insert(0, "None");
            return ruleTypes;
        }

        private bool ContainsRule(string ruleName)
        {
            if (ruleName == this.Name)
            {
                return false;
            }

            return this.NodeRule?.Rules?.Any(rule => rule.Name == ruleName) ?? false;
        }
    }
}
