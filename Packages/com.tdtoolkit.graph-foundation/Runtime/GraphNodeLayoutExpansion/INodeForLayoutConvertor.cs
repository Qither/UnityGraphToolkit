namespace GraphNodeLayoutExpansion.Runtime
{
    public interface INodeForLayoutConvertor
    {
        /// <summary>
        /// 节点间的距离
        /// </summary>
        float SiblingDistance { get; }

        object                         PrimRootNode   { get; }
        NodeAutoLayoutBuilder.TreeNode LayoutRootNode { get; }

        INodeForLayoutConvertor        Init(object primRootNode, string portDataName);
        NodeAutoLayoutBuilder.TreeNode PrimNode2LayoutNode();
        void                           LayoutNode2PrimNode();
    }
}