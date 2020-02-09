using BTree;
using System;
using System.Collections.Generic;

namespace BTree
{
    class Program
    {
        static void Main(string[] args)
        {
            Random r = new Random();

            //Random r = new Random();
            List<int> used = new List<int>();
            var tree = new BTree(3);
            for (int i = 0; i < 50; i++)
            {
                int newVal;
                do
                {
                    newVal = r.Next(0, 50);
                } while (used.Contains(newVal));
                used.Add(newVal);
                tree.Insert(newVal);
                //Console.WriteLine(tree);
                //tree.Print(tree.Root, 0);
                //Console.WriteLine($"-------{i}------- Dodano: {newVal}");
            }
            Console.WriteLine("Before saving to file.");
            Console.WriteLine(tree);
            BTree.WriteToBinaryFile<BTree>("xd", tree);
            var tree2 = BTree.ReadFromBinaryFile<BTree>("xd");
            Console.WriteLine("After reading from file.");
            Console.WriteLine(tree2);
            int odp;
            do
            {
                Console.WriteLine("Please select option.");
                Console.WriteLine("1. Add element, 2. Remove element, 3.Print tree, 4. Exit.");
                odp = Int32.Parse(Console.ReadLine());
                switch (odp)
                {
                    case 1:
                        try
                        {
                            Console.Write("Enter value to add: ");
                            int x = Int32.Parse(Console.ReadLine());
                            tree.Insert(x);
                        }catch(Exception e) { }
                        
                        break;
                    case 2:
                        try
                        {
                            Console.Write("Enter value to remove: ");
                            int y = Int32.Parse(Console.ReadLine());
                            tree.Delete(y);
                        }
                        catch (Exception e) { }
                        
                        break;
                    case 3:
                        Console.WriteLine(tree);
                        break;
                    default:
                        return;
                }
            } while (odp != 4);
            //for (int i = 0; i < 50; i++)
            //{
            //    int val = used[i];
            //    tree.Delete(val);
            //    Console.WriteLine(tree);
            //    Console.WriteLine($"-------{i}------- Usunięto: {val}");
            //}

            Console.ReadKey();
        }
    }
}
