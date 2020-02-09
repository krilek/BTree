namespace BTree
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Serializable]
    public class Node
    {
        private readonly int degree;
        public Node(int degree)
        {
            this.degree = degree;
            this.Children = new List<Node>(degree);
            this.Entries = new List<int>(degree);
        }

        public List<Node> Children { get; set; }

        public List<int> Entries { get; set; }

        public bool IsLeaf
        {
            get
            {
                return this.Children.Count == 0;
            }
        }

        public bool HasReachedMaxEntries
        {
            get
            {
                return this.Entries.Count == (2 * this.degree) - 1;
            }
        }

        public bool HasReachedMinEntries
        {
            get
            {
                return this.Entries.Count == this.degree - 1;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Entries.Count; i++)
            {
                sb.Append(Entries[i]);
                if (i != Entries.Count - 1)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }
    }
}
