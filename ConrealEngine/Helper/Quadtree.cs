using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConrealEngine.Helper
{ 
    public struct QuadtreeBounds
    {
        public IntVector2 TLCorner { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public List<QuadtreeBounds> Subdivide()
        {
            List<QuadtreeBounds> res = new List<QuadtreeBounds>();

            int subExtentWidth = Width / 2;
            int subExtentHeight = Height / 2;

            QuadtreeBounds t1 = new QuadtreeBounds();

            t1.TLCorner = new IntVector2(TLCorner.X, TLCorner.Y);
            t1.Width = subExtentWidth;
            t1.Height = subExtentHeight;

            QuadtreeBounds t2 = new QuadtreeBounds();

            t2.TLCorner = new IntVector2(TLCorner.X + subExtentWidth, TLCorner.Y);
            t2.Width = subExtentWidth;
            t2.Height = subExtentHeight;

            QuadtreeBounds t3 = new QuadtreeBounds();

            t3.TLCorner = new IntVector2(TLCorner.X, TLCorner.Y + subExtentHeight);
            t3.Width = subExtentWidth;
            t3.Height = subExtentHeight;

            QuadtreeBounds t4 = new QuadtreeBounds();

            t4.TLCorner = new IntVector2(TLCorner.X + subExtentWidth, TLCorner.Y + subExtentHeight);
            t4.Width = subExtentWidth;
            t4.Height = subExtentHeight;

            res.Add(t1);
            res.Add(t2);
            res.Add(t3);
            res.Add(t4);

            return res;
        }
    }

    public struct RenderQuadtreeData
    {
        public char Char { get; set; }
        public ConsoleColor BGColor { get; set; }
        public ConsoleColor FGColor { get; set; }
        public IntVector2 Position { get; set; }
    }

    public class RenderQuadtreeNode
    {
        private QuadtreeBounds bounds;
        private uint level;
        private RenderQuadtreeNode parent;
        private bool empty;
        private char[,] elements;

        public RenderQuadtreeNode(RenderQuadtreeNode parent, QuadtreeBounds bounds, uint level)
        {
            this.bounds = bounds;
            this.level = level;
            this.parent = parent;
            this.empty = true;

            elements = new char[bounds.Width, bounds.Height];

            for(int y = 0; y < Elements.GetLength(1); y++)
            {
                for (int x = 0; x < Elements.GetLength(0); x++)
                {
                    Elements[x,y] = ' ';
                }
            }
        }

        public List<RenderQuadtreeNode> Children { get; set; }
        public RenderQuadtreeNode Parent { get { return parent; } }
        public QuadtreeBounds Bounds { get { return bounds; } }
        public uint Level { get { return level; } }
        public char[,] Elements
        {
            get { return elements; }
        }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }
        public bool Empty { get { return empty; } }

        public void AddData(RenderQuadtreeData dataToInsert)
        {
            IntVector2 posToAdd = dataToInsert.Position - Bounds.TLCorner;

            if(posToAdd.X > 0 && posToAdd.X < Elements.GetLength(0) && posToAdd.Y > 0 && posToAdd.Y < Elements.GetLength(1))
            {
                if (dataToInsert.Char != ' ')
                    empty = false;

                Elements[posToAdd.X, posToAdd.Y] = dataToInsert.Char;
            }
        }

        public static bool operator ==(RenderQuadtreeNode n1, RenderQuadtreeNode n2)
        {
            if(n1.Empty && n2.Empty)
            {
                return n1.BackgroundColor == n2.BackgroundColor;
            }
            else
            {
                return n1.BackgroundColor == n2.BackgroundColor && n1.ForegroundColor == n2.ForegroundColor;
            }
        }

        public static bool operator !=(RenderQuadtreeNode n1, RenderQuadtreeNode n2)
        {
            if (n1.Empty && n2.Empty)
            {
                return n1.BackgroundColor != n2.BackgroundColor;
            }
            else
            {
                return n1.BackgroundColor != n2.BackgroundColor || n1.ForegroundColor != n2.ForegroundColor;
            }
        }
    }

    public class RenderQuadtree
    {
        private RenderQuadtreeNode root;
        private List<RenderQuadtreeNode> leafs = new List<RenderQuadtreeNode>();
        private uint maxRes;
        private QuadtreeBounds bounds;

        public RenderQuadtreeNode Root { get { return root; } }
        public List<RenderQuadtreeNode> Leafs { get; set; }
        public uint MaxRes { get { return maxRes; } }
        public QuadtreeBounds Bounds { get { return bounds; } }

        public RenderQuadtree(QuadtreeBounds bounds, uint maxSubLevel)
        {
            this.bounds = bounds;
            this.maxRes = maxSubLevel;

            root = new RenderQuadtreeNode(null, bounds, 0);
        }

        /*public AddElement(IntVector2 pos, T data)
        {

        }*/

        /*private void AddElementToNode(RenderQuadtreeNode nodeToEdit, T dataToAdd, IntVector2 pos, out RenderQuadtreeNode editedNode)
        {

        }*/

        private void ClearChildren(RenderQuadtreeNode parentNode, uint treeLevel = 0)
        {

        }

        private void OptimizeTree(RenderQuadtreeNode startNode)
        {

        }

        private void MakeToLeaf(RenderQuadtreeNode node, RenderQuadtreeData dataToInsert)
        {
            if (node.Children.Count > 0)
                return;

            node.AddData(dataToInsert);

            AddToLeafsList(node);
        }

        private void SubdivideNode(RenderQuadtreeNode nodeToSubdivide)
        {

        }

        private void AddToLeafsList(RenderQuadtreeNode node)
        {

        }

        private void RemoveFromLeafsList(RenderQuadtreeNode node)
        {

        }
    }
}
