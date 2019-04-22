using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BTree
{
    class Tree
    {
        public readonly int t;
        public readonly int MaxAmountOfKeys;
        public readonly int MaxAmountOfChilds;
        public Node Root { get; set; }
        public Tree(int degreeOfTree, int key)
        {
            t = degreeOfTree;
            this.MaxAmountOfChilds = 2 * degreeOfTree;
            this.MaxAmountOfKeys = 2 * degreeOfTree - 1;
            Root = new Node(MaxAmountOfKeys, MaxAmountOfChilds);
            Root.Leaf = true;
            Root.k[0] = key;
            Root.n++;
        }
        public Node SearchKey(int key)
        {
            return SearchKey(Root, key);
        }

        private Node SearchKey(Node checkedNode, int key)
        {
            int i = 0;
            while (i < checkedNode.n && key > checkedNode.k[i])  
            {
                i++;
            }
            if(i<=checkedNode.n && key == checkedNode.k[i])
            {
                return checkedNode;
            }
            if (checkedNode.Leaf)
            {
                return null;
            }
            
            return SearchKey(checkedNode.c[i], key);
        }

        public void SplitChild(Node x, Node y)
        {
            int i = Array.IndexOf(x.c, y);

            Node z = new Node(MaxAmountOfKeys, MaxAmountOfChilds)
            {
                Leaf = y.Leaf,
                n = t-1
            };
            for (int j = 0; j < t-1; j++)
            {
                z.k[j] = y.k[j + t];
            }
            if (!y.Leaf)
            {
                for (int j = 0; j < t; j++)
                {
                    z.c[j] = y.c[j + t];
                }
            }
            y.n = t - 1;

            for (int j = x.n; j > i; j--)
            {
                x.c[j + 1] = x.c[j];
            }
            x.c[i + 1] = z;
            for (int j = x.n - 1; j > i; j--)
            {
                x.k[j + 1] = x.k[j];
            }
            x.k[i] = y.k[t-1];
            y.k[t - 1] = 0;
            for(int j = 0; j < t - 1; j++)
            {
                y.k[j + t] = 0;
            }
            x.n++;
        }

        public void Insert(int k)
        {
            Node r = Root;
            if(r.n == 2 * t - 1)
            {
                Node s = new Node(MaxAmountOfKeys, MaxAmountOfChilds);
                Root = s;
                s.Leaf = false;
                s.n = 0;
                s.c[0] = r;
                SplitChild(s, r);
                Insert(s, k);
            }
            else
            {
                Insert(r, k);
            }
        }

        private void Insert(Node x, int k)
        {
            // node has empty space
            int i = x.n;
            if (x.Leaf)
            {
                while (i >= 0 && k<x.k[i])
                {
                    x.k[i + 1] = x.k[i];
                    i--;
                }
                x.k[i] = k;
                x.n = x.n + 1;
            }
            else
            {
                while (i >= 0 && k<x.k[i])
                {
                    i--;
                }
                i++;
                Node next = x.c[i];
                if(x.c[i].n == 2 * t - 1)
                {
                    SplitChild(x, x.c[i]);
                    if (k > x.k[i])
                    {
                        i++;
                    }
                }
                Insert(x.c[i], k);
            }
        }
        //public static readonly int T = 3;
        //public FileStream OutputFile { get; set; }
        //public void Save(int i, Node n)
        //{
        //    OutputFile.Seek(i * Marshal.SizeOf(typeof(Node)), SeekOrigin.Begin);
        //    OutputFile.Write()
        //}
    }
    
}
