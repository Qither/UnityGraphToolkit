using GraphProcessor;
using RedDotSystem.Editor.Node;
using RedDotSystem.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedDotSystem.Editor
{
    public static class RedDotHelper
    {
        public static readonly Dictionary<string, Type> RedDotMultiNodeDataTypes = new Dictionary<string, Type>();
        
        public static readonly Dictionary<string, Type> RedDotRuleTypes = new Dictionary<string, Type>();
        
        public static void Refresh(this RedDotNode redDotNode, string prevNode = "")
        {
            if (string.IsNullOrEmpty(prevNode))
            {
                NodePort inputNodes = redDotNode.inputPorts?.FirstOrDefault(x => x.fieldName.Equals("WordInput"));
                while (inputNodes != null && inputNodes.GetEdges().Count > 0)
                {
                    BaseNode baseNode = inputNodes.GetEdges()[0].outputNode;
                    prevNode = baseNode.GetCustomName() + (string.IsNullOrEmpty(prevNode) ? "" : "\n") + prevNode;
                    inputNodes = baseNode.inputPorts
                        .First(x => x.fieldName.Equals("WordInput"));
                }
            }

            redDotNode.Data.Key = prevNode + (string.IsNullOrEmpty(prevNode) ? "" : "\n") + redDotNode.GetCustomName();
            prevNode            = redDotNode.Data.Key;

            BaseNode[] outputNodes = redDotNode.outputPorts?.FirstOrDefault(x => x.fieldName.Equals("WordOutput"))?.GetEdges()
                .Select(x => x.inputNode).ToArray();
            
            if (outputNodes == null) return;

                foreach (BaseNode node in outputNodes)
            {
                if (node is RedDotNode childNode)
                {
                    childNode.Refresh(prevNode);
                }
            }
        }
        
        public static void CheckChildNodeCustomName(this RedDotNode redDotNode)
        {
            BaseNode[] outputNodes = redDotNode.outputPorts?.FirstOrDefault(x => x.fieldName.Equals("WordOutput"))?.GetEdges()
                .Select(x => x.inputNode).ToArray();
            if (outputNodes == null) return;

            foreach (BaseNode outputNode in outputNodes)
            {
                outputNode.ClearMessages();
                foreach (BaseNode node in outputNodes)
                {
                    if (outputNode.Equals(node)) continue;
                    if (!outputNode.GetCustomName().Equals(node.GetCustomName())) continue;
                    
                    // outputNode.AddMessage($"RedDotNodeView: {outputNode.GetCustomName()} is already exist.", NodeMessageType.Error);
                    // node.AddMessage($"RedDotNodeView: {node.GetCustomName()} is already exist.", NodeMessageType.Error);

                    outputNode.AddMessage($"红点系统: {outputNode.GetCustomName()} 节点已存在。", NodeMessageType.Error);
                    node.AddMessage($"红点系统: {node.GetCustomName()} 节点已存在。", NodeMessageType.Error);
                }
            }
        }

        public static RedDotData CreateRedDotData(this BaseNode baseNode, string prefix = "",
            char separator = RedDotConst.RED_DOT_NODE_NAME_SEPARATOR, RedDotData preRedDotData = null)
        {
            List<RedDotData> nextData      = new List<RedDotData>();
            RedDotData       curRedDotData = null;

            string curRedDotName = preRedDotData?.NodeName;
            switch (baseNode)
            {
                case RedDotMultiNode multiNode:
                    if (!multiNode.Data.MultiNodeData.Equals("None") &&
                        !string.IsNullOrEmpty(multiNode.Data.MultiNodeData))
                    {
                        if (RedDotMultiNodeDataTypes.TryGetValue(multiNode.Data.MultiNodeData,
                                out Type redDotMultiDataType))
                        {
                            curRedDotData = Activator.CreateInstance(redDotMultiDataType,
                                curRedDotName, preRedDotData, multiNode.Data.System, nextData, CreateRedDotRules(baseNode)) as RedDotMultiData;
                        }
                    }

                    break;
                case RedDotNode redDotNode:
                    curRedDotName = string.Format($"{prefix}{separator}{redDotNode.Data.Key.Replace('\n', separator)}");
                    curRedDotData = new RedDotSingeData(curRedDotName, preRedDotData, redDotNode.Data.System, nextData,
                        CreateLinkDataNames(baseNode, prefix, separator), CreateRedDotRules(baseNode));
                    break;
            }

            BaseNode[] outputNodes = baseNode.outputPorts?.FirstOrDefault(x => x.fieldName.Equals("WordOutput"))
                ?.GetEdges()
                .Select(x => x.inputNode).ToArray();

            if (outputNodes == null || outputNodes.Length == 0 || curRedDotData == null) return curRedDotData;

            nextData.AddRange(outputNodes
                .Select(childNode => childNode.CreateRedDotData(prefix, separator, curRedDotData))
                .Where(childRedDotData => childRedDotData != null));

            return curRedDotData;
        }

        private static LinkedList<RedDotRule> CreateRedDotRules(this BaseNode baseNode)
        {
            RuleList rules = baseNode switch
            {
                RedDotMultiNode multiNode => multiNode.Rule.Rules,
                RedDotNode redDotNode     => redDotNode.Rule.Rules,
                _                         => null
            };

            if (rules == null || rules.Count == 0) return null;

            LinkedList<RedDotRule> redDotRules = new LinkedList<RedDotRule>();
            foreach (RuleData ruleData in rules)
            {
                if (RedDotRuleTypes.TryGetValue(ruleData.Name, out Type redDotRuleType))
                {
                    RedDotRule redDotRule = Activator.CreateInstance(redDotRuleType) as RedDotRule;
                    redDotRules.AddLast(redDotRule);
                }
            }

            return redDotRules;
        }

        private static RedDotSingeData.LinkDataNames CreateLinkDataNames(BaseNode baseNode, string prefix = "",
            char separator = RedDotConst.RED_DOT_NODE_NAME_SEPARATOR)
        {
            BaseNode[] linkOutputNodes = baseNode.outputPorts?.FirstOrDefault(x => x.fieldName.Equals("LinkOutput"))
                ?.GetEdges()
                .Select(x => x.inputNode).ToArray();

            BaseNode[] linkInputNodes = baseNode.inputPorts?.FirstOrDefault(x => x.fieldName.Equals("LinkInput"))
                ?.GetEdges()
                .Select(x => x.outputNode).ToArray();

            List<string> linkInputNodeNames = new List<string>();
            if (linkInputNodes != null && linkInputNodes.Length > 0)
            {
                foreach (BaseNode node in linkInputNodes)
                {
                    if (node is RedDotNode linkInputNode)
                    {
                        linkInputNodeNames.Add(
                            string.Format($"{prefix}{separator}{linkInputNode.Data.Key.Replace('\n', separator)}"));
                    }
                }
            }

            List<string> linkOutputNodeNames = new List<string>();
            if (linkOutputNodes != null && linkOutputNodes.Length > 0)
            {
                foreach (BaseNode node in linkOutputNodes)
                {
                    if (node is RedDotNode linkOutputNode)
                    {
                        linkOutputNodeNames.Add(
                            string.Format($"{prefix}{separator}{linkOutputNode.Data.Key.Replace('\n', separator)}"));
                    }
                }
            }

            return new RedDotSingeData.LinkDataNames(linkInputNodeNames, linkOutputNodeNames);
        }

        public static string GetFunctionName(this RedDotNode redDotNode,
            char separator = RedDotConst.RED_DOT_NODE_NAME_SEPARATOR)
        {
            string[] funcSplit = redDotNode.Data.Key.Split('\n');
            string   funcName  = $"{string.Join(separator.ToString(), funcSplit)}";
            return funcName;
        }
    }
}