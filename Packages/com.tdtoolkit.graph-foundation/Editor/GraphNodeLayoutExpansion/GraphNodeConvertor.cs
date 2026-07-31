using GraphNodeLayoutExpansion.Runtime;
using GraphProcessor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphNodeLayoutExpansion.Editor
{
    public class GraphNodeConvertor : INodeForLayoutConvertor
    {
        public virtual float SiblingDistance => 50;

        public object                         PrimRootNode   { get; private set; }
        public string                         PortDataName   { get; private set; }
        public NodeAutoLayoutBuilder.TreeNode LayoutRootNode { get; private set; }

        public INodeForLayoutConvertor Init(object primRootNode, string portDataName)
        {
            this.PrimRootNode = primRootNode;
            this.PortDataName = portDataName;
            
            return this;
        }

        public NodeAutoLayoutBuilder.TreeNode PrimNode2LayoutNode()
        {
            if (!(this.PrimRootNode is BaseNodeView graphNodeViewBase))
                return null;

            this.LayoutRootNode = new NodeAutoLayoutBuilder.TreeNode(
                graphNodeViewBase.layout.height + this.SiblingDistance,
                graphNodeViewBase.layout.width,
                graphNodeViewBase.layout.y,
                NodeAutoLayoutBuilder.CalculateMode.Horizontal | NodeAutoLayoutBuilder.CalculateMode.Positive);

            this.Convert2LayoutNode(graphNodeViewBase,
                this.LayoutRootNode, graphNodeViewBase.layout.y + graphNodeViewBase.layout.width,
                NodeAutoLayoutBuilder.CalculateMode.Horizontal |
                NodeAutoLayoutBuilder.CalculateMode.Positive);

            return this.LayoutRootNode;
        }
        
        private void Convert2LayoutNode(BaseNodeView rootPrimNode,
            NodeAutoLayoutBuilder.TreeNode rootLayoutNode, float lastHeightPoint,
            NodeAutoLayoutBuilder.CalculateMode calculateMode)
        {
            PortView outputPortView = rootPrimNode.outputPortViews.FirstOrDefault(x => x.portData.displayName == this.PortDataName);
            if (!(outputPortView?.GetEdges()?.Count > 0)) return;

            foreach (EdgeView edgeView in outputPortView.GetEdges())
            {
                if (!(edgeView.input.node is BaseNodeView childNode)) continue;

                NodeAutoLayoutBuilder.TreeNode childLayoutNode =
                    new NodeAutoLayoutBuilder.TreeNode(childNode.layout.height + this.SiblingDistance,
                        childNode.layout.width,
                        lastHeightPoint + this.SiblingDistance, calculateMode);
                rootLayoutNode.AddChild(childLayoutNode);
                this.Convert2LayoutNode(childNode, childLayoutNode,
                    lastHeightPoint + this.SiblingDistance + childNode.layout.width,
                    calculateMode);
            }
        }

        public void LayoutNode2PrimNode()
        {
            Vector2 calculateRootResult = this.LayoutRootNode.GetPos();

            BaseNodeView root = this.PrimRootNode as BaseNodeView;
            root?.SetPosition(new Rect(calculateRootResult, Vector2.zero));

            this.Convert2PrimNode(this.PrimRootNode as BaseNodeView, this.LayoutRootNode);
        }
        
        private void Convert2PrimNode(BaseNodeView rootPrimNode,
            NodeAutoLayoutBuilder.TreeNode rootLayoutNode)
        {
            PortView outputPortView = rootPrimNode.outputPortViews.FirstOrDefault(x => x.portData.displayName == this.PortDataName);
            if (!(outputPortView?.GetEdges()?.Count > 0)) return;
            
            List<EdgeView> children = outputPortView.GetEdges().ToList();
            for (int i = 0; i < children.Count; i++)
            {
                EdgeView edgeView = children[i];
                if (!(edgeView.input.node is BaseNodeView childNode)) continue;
                
                Vector2 calculateResult = rootLayoutNode.Children[i].GetPos();

                childNode.SetPosition(new Rect(calculateResult, Vector2.zero));

                this.Convert2PrimNode(childNode, rootLayoutNode.Children[i]);
            }
        }
    }
}