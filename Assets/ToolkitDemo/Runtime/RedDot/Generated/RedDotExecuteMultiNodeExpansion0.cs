using RedDotSystem.Runtime;
using ToolkitDemo.RedDotDemo;

namespace ToolkitDemo.RedDot.Generated
{
    public partial class RedDotExecuteNode
    {
        /// <summary>
        /// Leaf execution node with a custom rule
        /// </summary>
        public void Inbox_Rewards(TrieNode<(RedDotData nodeData, int nodeValue)> node)
        {
            int value = node.NodeValue == "ALPHA"
                ? DemoRedDotState.PrimaryCount
                : DemoRedDotState.SecondaryCount;
            node.Data = (node.Data.nodeData, value);
        }

    }
}
