using System;
using UnityEngine;

namespace RedDotSystem.Runtime
{
    [Serializable]
    public class RedDotLinkRule: RedDotRule
    {
        protected override void OnExecuteAfter(TrieNode<(RedDotData nodeData, int nodeValue)> node)
        {
            if (!(this.Order.Data.nodeData is RedDotSingeData singeData)) return;

            int nodeValue = 0;
            singeData.LinkData.NextNodeData.ForEach(nextNodeName =>
            {
                TrieNode<(RedDotData nodeData, int nodeValue)> nextNode = this.RedDotService.GetNode(nextNodeName);
                if (nextNode != null)
                {
                    this.RedDotService.ExecuteNode(nextNode);
                    nodeValue += nextNode.Data.nodeValue;
                }
                else
                {
                    Debug.Log($"RedDotLinkRule: OnExecuteAfter nextNode {nextNodeName} is null.");
                }
            });
            this.Order.Data = (this.Order.Data.nodeData, nodeValue);
        }
    }
}