using System;

namespace BTree
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Tree t = new Tree(3, 3);
            t.Insert(5);
            t.Insert(6);
            t.Insert(7);
            t.Insert(8);
            t.Insert(9);
            //Node x = t.SearchKey(7);
        }
    }
}
