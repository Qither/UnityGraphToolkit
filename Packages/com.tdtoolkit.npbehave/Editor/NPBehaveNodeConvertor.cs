using GraphNodeLayoutExpansion.Runtime;
using GraphProcessor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NPBehaveEditor
{
    public class NPBehaveNodeConvertor : INodeForLayoutConvertor
    {
        public float                          SiblingDistance => 50;
        
        public object                         PrimRootNode   { get; private set; }
        
        public string                         PortDataName   { get; private set; }
        public NodeAutoLayoutBuilder.TreeNode LayoutRootNode { get; private set; }
        
        public INodeForLayoutConvertor        Init(object primRootNode, string portDataName)
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
                graphNodeViewBase.layout.size.x + this.SiblingDistance,
                graphNodeViewBase.layout.size.y,
                graphNodeViewBase.layout.position.y,
                NodeAutoLayoutBuilder.CalculateMode.Vertical | NodeAutoLayoutBuilder.CalculateMode.Positive);

            this.Convert2LayoutNode(graphNodeViewBase,
                this.LayoutRootNode, graphNodeViewBase.layout.position.y + graphNodeViewBase.layout.size.y,
                NodeAutoLayoutBuilder.CalculateMode.Vertical |
                NodeAutoLayoutBuilder.CalculateMode.Positive);

            return this.LayoutRootNode;
        }

        private void Convert2LayoutNode(BaseNodeView rootPrimNode,
            NodeAutoLayoutBuilder.TreeNode rootLayoutNode, float lastHeightPoint,
            NodeAutoLayoutBuilder.CalculateMode calculateMode)
        {
            PortView outputPortView = rootPrimNode.outputPortViews.FirstOrDefault(x => x.portData.displayName == this.PortDataName);
            if (!(outputPortView?.GetEdges()?.Count > 0)) return;

            List<BaseNodeView> children = outputPortView.GetEdges().Select(x => x.input.node as BaseNodeView).ToList();
            children.Sort((x, y) => x.layout.position.x.CompareTo(y.layout.position.x));
            
            foreach (BaseNodeView childNode in children)
            {
                NodeAutoLayoutBuilder.TreeNode childLayoutNode =
                    new NodeAutoLayoutBuilder.TreeNode(childNode.layout.size.x + this.SiblingDistance,
                        childNode.layout.size.y,
                        lastHeightPoint + this.SiblingDistance, calculateMode);
                rootLayoutNode.AddChild(childLayoutNode);
                this.Convert2LayoutNode(childNode, childLayoutNode,
                    lastHeightPoint + this.SiblingDistance + childNode.layout.size.y,
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
            
            List<BaseNodeView> children = outputPortView.GetEdges().Select(x => x.input.node as BaseNodeView).ToList();
            children.Sort((x, y) => x.layout.position.x.CompareTo(y.layout.position.x));
            for (int i = 0; i < children.Count; i++)
            {
                BaseNodeView childNode       = children[i];
                Vector2      calculateResult = rootLayoutNode.Children[i].GetPos();

                childNode.SetPosition(new Rect(calculateResult, Vector2.zero));
                this.Convert2PrimNode(childNode, rootLayoutNode.Children[i]);
            }
        }
    }
}