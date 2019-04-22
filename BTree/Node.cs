using System;
using System.Collections.Generic;
using System.Text;

namespace BTree
{
    class Node
    {
        public int n;
        public bool Leaf { get; set; }
        public int[] k;
        public Node[] c;
        public Node(int keysAmount, int childsAmount)
        {
            n = 0;
            k = new int[keysAmount];
            c = new Node[childsAmount];
            Leaf = false;
        }

    }
}
