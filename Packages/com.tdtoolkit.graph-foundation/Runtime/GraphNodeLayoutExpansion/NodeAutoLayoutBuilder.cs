using System.Collections.Generic;
using UnityEngine;

namespace GraphNodeLayoutExpansion.Runtime
{
    public class NodeAutoLayoutBuilder
    {
        [System.Flags]
        public enum CalculateMode
        {
            // 水平计算模式
            Horizontal = 1,

            // 垂直计算模式
            Vertical = Horizontal << 1,

            // 从上到下，从左到右
            Positive = Horizontal << 2,

            // 从下到上，从右到左
            Negative = Horizontal << 3
        }

        public class TreeNode
        {
            public float          W,    H;    // Width and height.
            public float          X,    Y, Prelim, Mod, Shift, Change;
            public TreeNode       TL,   TR;   // Left and right thread.                      
            public TreeNode       EL,   ER;   // Extreme left and right nodes. 
            public float          Msel, Mser; // Sum of modifiers at the extreme nodes. 
            
            public List<TreeNode> Children = new List<TreeNode>();

            public int ChildrenCount =>
                this.Children.Count; // Array of children and number of children. 

            public CalculateMode CalculateMode;

            public TreeNode(float w, float h, float y,
                CalculateMode calculateMode = CalculateMode.Vertical | CalculateMode.Positive)
            {
                this.W             = w;
                this.H             = h;
                this.Y             = y;
                this.CalculateMode = calculateMode;
            }

            public void AddChild(TreeNode child)
            {
                this.Children.Add(child);
            }

            public Vector2 GetPos()
            {
                Vector2 calculateResult = new Vector2(this.X, this.Y);

                // 根节点位于原点，绕原点进行旋转(绕原点的旋转矩阵)，并对节点位置进行修正（因为宽高翻转）
                if (this.CalculateMode == (CalculateMode.Horizontal | CalculateMode.Negative))
                {
                    Vector2 temp = calculateResult;
                    temp.x          = -calculateResult.y - this.H;
                    temp.y          = calculateResult.x;
                    calculateResult = temp;
                }

                if (this.CalculateMode == (CalculateMode.Horizontal | CalculateMode.Positive))
                {
                    Vector2 temp = calculateResult;
                    temp.x          = calculateResult.y;
                    temp.y          = -calculateResult.x - this.W;
                    calculateResult = temp;
                }

                if (this.CalculateMode == (CalculateMode.Vertical | CalculateMode.Negative))
                {
                    calculateResult.y = -calculateResult.y - this.H;
                }

                return calculateResult;
            }
        }

        public static void Layout(INodeForLayoutConvertor nodeForLayoutConvertor)
        {
            if (nodeForLayoutConvertor.PrimNode2LayoutNode() == null)
            {
                return;
            }

            FirstWalk(nodeForLayoutConvertor.LayoutRootNode);
            SecondWalk(nodeForLayoutConvertor.LayoutRootNode, 0);

            nodeForLayoutConvertor.LayoutNode2PrimNode();
        }

        private static void FirstWalk(TreeNode t)
        {
            if (t.ChildrenCount == 0)
            {
                SetExtremes(t);
                return;
            }

            FirstWalk(t.Children[0]);
            // Create siblings in contour minimal vertical coordinate and index list.
            IYL ih = UpdateIYL(Bottom(t.Children[0].EL), 0, null);
            for (int i = 1; i < t.ChildrenCount; i++)
            {
                FirstWalk(t.Children[i]);
                // Store lowest vertical coordinate while extreme nodes still point in current subtree.
                float minY = Bottom(t.Children[i].ER);
                Separate(t, i, ih);
                ih = UpdateIYL(minY, i, ih);
            }

            PositionRoot(t);
            SetExtremes(t);
        }

        private static void SetExtremes(TreeNode t)
        {
            if (t.ChildrenCount == 0)
            {
                t.EL   = t;
                t.ER   = t;
                t.Msel = t.Mser = 0;
            }
            else
            {
                t.EL   = t.Children[0].EL;
                t.Msel = t.Children[0].Msel;
                t.ER   = t.Children[t.ChildrenCount - 1].ER;
                t.Mser = t.Children[t.ChildrenCount - 1].Mser;
            }
        }

        private static void Separate(TreeNode t, int i, IYL ih)
        {
            // Right contour node of left siblings and its sum of modfiers.
            TreeNode sr   = t.Children[i - 1];
            float    mssr = sr.Mod;
            // Left contour node of current subtree and its sum of modfiers.
            TreeNode cl   = t.Children[i];
            float    mscl = cl.Mod;
            while (sr != null && cl != null)
            {
                if (Bottom(sr) > ih.LowY)
                {
                    ih = ih.Next;
                }

                // How far to the left of the right side of sr is the left side of cl?
                float dist = mssr + sr.Prelim + sr.W - (mscl + cl.Prelim);
                if (dist > 0)
                {
                    mscl += dist;
                    MoveSubtree(t, i, ih.Index, dist);
                }

                float sy = Bottom(sr), cy = Bottom(cl);
                // Advance highest node(s) and sum(s) of modifiers
                if (sy <= cy)
                {
                    sr = NextRightContour(sr);
                    if (sr != null)
                    {
                        mssr += sr.Mod;
                    }
                }

                if (sy >= cy)
                {
                    cl = NextLeftContour(cl);
                    if (cl != null)
                    {
                        mscl += cl.Mod;
                    }
                }
            }

            // Set threads and update extreme nodes.
            // In the first case, the current subtree must be taller than the left siblings.
            if (sr == null && cl != null)
            {
                SetLeftThread(t, i, cl, mscl);
            }
            // In this case, the left siblings must be taller than the current subtree.
            else if (sr != null && cl == null)
            {
                SetRightThread(t, i, sr, mssr);
            }
        }

        private static void MoveSubtree(TreeNode t, int i, int si, float dist)
        {
            // Move subtree by changing mod.
            t.Children[i].Mod  += dist;
            t.Children[i].Msel += dist;
            t.Children[i].Mser += dist;
            DistributeExtra(t, i, si, dist);
        }

        private static TreeNode NextLeftContour(TreeNode t) { return t.ChildrenCount == 0 ? t.TL : t.Children[0]; }

        private static TreeNode NextRightContour(TreeNode t)
        {
            return t.ChildrenCount == 0 ? t.TR : t.Children[t.ChildrenCount - 1];
        }

        private static float Bottom(TreeNode t) { return t.Y + t.H; }

        private static void SetLeftThread(TreeNode t, int i, TreeNode cl, float modsumcl)
        {
            TreeNode li = t.Children[0].EL;
            li.TL = cl;
            // Change mod so that the sum of modifier after following thread is correct.
            float diff = modsumcl - cl.Mod - t.Children[0].Msel;
            li.Mod += diff;
            // Change preliminary x coordinate so that the node does not move.
            li.Prelim -= diff;
            // Update extreme node and its sum of modifiers.
            t.Children[0].EL   = t.Children[i].EL;
            t.Children[0].Msel = t.Children[i].Msel;
        }

        // Symmetrical to setLeftThread.
        private static void SetRightThread(TreeNode t, int i, TreeNode sr, float modsumsr)
        {
            TreeNode ri = t.Children[i].ER;
            ri.TR = sr;
            float diff = modsumsr - sr.Mod - t.Children[i].Mser;
            ri.Mod             += diff;
            ri.Prelim          -= diff;
            t.Children[i].ER   =  t.Children[i - 1].ER;
            t.Children[i].Mser =  t.Children[i - 1].Mser;
        }

        private static void PositionRoot(TreeNode t)
        {
            // Position root between children, taking into account their mod.
            t.Prelim = ((t.Children[0].Prelim + t.Children[0].Mod + t.Children[t.ChildrenCount - 1].Mod +
                         t.Children[t.ChildrenCount - 1].Prelim + t.Children[t.ChildrenCount - 1].W) / 2) - (t.W / 2);
        }

        private static void SecondWalk(TreeNode t, float modsum)
        {
            modsum += t.Mod;
            // Set absolute (non-relative) horizontal coordinate.
            t.X = t.Prelim + modsum;
            AddChildSpacing(t);
            for (int i = 0; i < t.ChildrenCount; i++)
            {
                SecondWalk(t.Children[i], modsum);
            }
        }

        private static void DistributeExtra(TreeNode t, int i, int si, float dist)
        {
            // Are there intermediate children?
            if (si != i - 1)
            {
                float nr = i - si;
                t.Children[si + 1].Shift += dist / nr;
                t.Children[i].Shift      -= dist / nr;
                t.Children[i].Change     -= dist - (dist / nr);
            }
        }

        // Process change and shift to add intermediate spacing to mod.
        private static void AddChildSpacing(TreeNode t)
        {
            float d = 0, modsumdelta = 0;
            for (int i = 0; i < t.ChildrenCount; i++)
            {
                d                 += t.Children[i].Shift;
                modsumdelta       += d + t.Children[i].Change;
                t.Children[i].Mod += modsumdelta;
            }
        }

        // A linked list of the indexes of left siblings and their lowest vertical coordinate.
        private class IYL
        {
            public float LowY;
            public int   Index;
            public IYL   Next;

            public IYL(float lowY, int index, IYL next)
            {
                this.LowY  = lowY;
                this.Index = index;
                this.Next   = next;
            }
        }

        private static IYL UpdateIYL(float minY, int i, IYL ih)
        {
            // Remove siblings that are hidden by the new subtree.
            while (ih != null && minY >= ih.LowY)
            {
                ih = ih.Next;
            }

            // Prepend the new subtree.
            return new IYL(minY, i, ih);
        }
    }
}