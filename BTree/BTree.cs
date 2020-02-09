namespace BTree
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class BTree
    {
        public BTree(int degree)
        {
            Root = new Node(degree);
            Degree = degree;
        }

        public Node Root { get; private set; }

        public int Degree { get; private set; }

        public int Search(int key)
        {
            return Search(Root, key);
        }

        public void Insert(int newKey)
        {
            // Add to root when there is space
            if (!Root.HasReachedMaxEntries)
            {
                InsertNonFull(Root, newKey);
                return;
            }

            // Root full, split and look into subtrees
            Node oldRoot = Root;
            Root = new Node(Degree);
            Root.Children.Add(oldRoot);
            SplitChild(Root, 0, oldRoot);
            InsertNonFull(Root, newKey);
        }

        public void Delete(int keyToDelete)
        {
            Delete(Root, keyToDelete);

            // Fix tree when all keys of root was removed, root should have only one child if it is not leaf
            if (Root.Entries.Count == 0 && !Root.IsLeaf)
            {
                Root = Root.Children.Single();
            }
        }

        private void Delete(Node node, int keyToDelete)
        {
            // Find index of possible key to remove 
            int i = node.Entries.TakeWhile(entry => keyToDelete > entry).Count();

            // If key exists in this node, remove it
            if (i < node.Entries.Count && node.Entries[i] == keyToDelete)
            {
                DeleteKeyFromNode(node, keyToDelete, i);
                return;
            }

            // If not look into subtree
            if (!node.IsLeaf)
            {
                DeleteKeyFromSubtree(node, keyToDelete, i);
            }
        }

        private void DeleteKeyFromSubtree(Node parentNode, int keyToDelete, int subtreeIndexInNode)
        {
            Node childNode = parentNode.Children[subtreeIndexInNode];

            // check if child has enough childs, if yes merge or take from sibling
            if (childNode.HasReachedMinEntries)
            {
                int leftIndex = subtreeIndexInNode - 1;
                Node leftSibling = subtreeIndexInNode > 0 ? parentNode.Children[leftIndex] : null;

                int rightIndex = subtreeIndexInNode + 1;
                Node rightSibling = subtreeIndexInNode < parentNode.Children.Count - 1
                                                ? parentNode.Children[rightIndex]
                                                : null;

                if (leftSibling != null && leftSibling.Entries.Count > Degree - 1)
                {
                    // left sibling has enough to give to our parent and then parent to our child
                    childNode.Entries.Insert(0, parentNode.Entries[subtreeIndexInNode]);
                    parentNode.Entries[subtreeIndexInNode] = leftSibling.Entries.Last();
                    leftSibling.Entries.RemoveAt(leftSibling.Entries.Count - 1);

                    if (!leftSibling.IsLeaf)
                    {
                        childNode.Children.Insert(0, leftSibling.Children.Last());
                        leftSibling.Children.RemoveAt(leftSibling.Children.Count - 1);
                    }
                }
                else if (rightSibling != null && rightSibling.Entries.Count > Degree - 1)
                {
                    // same thing as above but right sibling has spare child
                    childNode.Entries.Add(parentNode.Entries[subtreeIndexInNode]);
                    parentNode.Entries[subtreeIndexInNode] = rightSibling.Entries.First();
                    rightSibling.Entries.RemoveAt(0);

                    if (!rightSibling.IsLeaf)
                    {
                        childNode.Children.Add(rightSibling.Children.First());
                        rightSibling.Children.RemoveAt(0);
                    }
                }
                else
                {
                    // merge
                    if (leftSibling != null)
                    {
                        childNode.Entries.Insert(0, parentNode.Entries[subtreeIndexInNode]);
                        System.Collections.Generic.List<int> oldEntries = childNode.Entries;
                        childNode.Entries = leftSibling.Entries;
                        childNode.Entries.AddRange(oldEntries);
                        if (!leftSibling.IsLeaf)
                        {
                            System.Collections.Generic.List<Node> oldChildren = childNode.Children;
                            childNode.Children = leftSibling.Children;
                            childNode.Children.AddRange(oldChildren);
                        }

                        parentNode.Children.RemoveAt(leftIndex);
                        parentNode.Entries.RemoveAt(subtreeIndexInNode);
                    }
                    else
                    {
                        childNode.Entries.Add(parentNode.Entries[subtreeIndexInNode]);
                        childNode.Entries.AddRange(rightSibling.Entries);
                        if (!rightSibling.IsLeaf)
                        {
                            childNode.Children.AddRange(rightSibling.Children);
                        }

                        parentNode.Children.RemoveAt(rightIndex);
                        parentNode.Entries.RemoveAt(subtreeIndexInNode);
                    }
                }
            }

            Delete(childNode, keyToDelete);
        }

        private void DeleteKeyFromNode(Node node, int keyToDelete, int keyIndexInNode)
        {
            // When on leaf, just remove it, degree violation doesn't happen
            if (node.IsLeaf)
            {
                node.Entries.RemoveAt(keyIndexInNode);
                return;
            }

            // Delete from right max key, push it to top if no problem with violation
            Node smallerChild = node.Children[keyIndexInNode];
            if (smallerChild.Entries.Count >= Degree)
            {
                int biggestVal = DeleteMax(smallerChild);
                node.Entries[keyIndexInNode] = biggestVal;
            }
            else
            {
                // Same thing for as above but with smallest
                Node biggerChild = node.Children[keyIndexInNode + 1];
                if (biggerChild.Entries.Count >= Degree)
                {
                    int smallestVal = DeleteMin(smallerChild);
                    node.Entries[keyIndexInNode] = smallestVal;
                }
                else
                {
                    // Merge if none of above happened
                    smallerChild.Entries.Add(node.Entries[keyIndexInNode]);
                    smallerChild.Entries.AddRange(biggerChild.Entries);
                    smallerChild.Children.AddRange(biggerChild.Children);
                    // Cleanup parent
                    node.Entries.RemoveAt(keyIndexInNode);
                    node.Children.RemoveAt(keyIndexInNode + 1);

                    Delete(smallerChild, keyToDelete);
                }
            }
        }

        private int DeleteMax(Node node)
        {
            if (node.IsLeaf)
            {
                int result = node.Entries[node.Entries.Count - 1];
                node.Entries.RemoveAt(node.Entries.Count - 1);
                return result;
            }

            return DeleteMax(node.Children.Last());
        }

        private int DeleteMin(Node node)
        {
            if (node.IsLeaf)
            {
                int result = node.Entries[0];
                node.Entries.RemoveAt(0);
                return result;
            }

            return DeleteMax(node.Children.First());
        }

        private int Search(Node node, int key)
        {
            int i = node.Entries.TakeWhile(entry => key > entry).Count();

            if (i < node.Entries.Count && node.Entries[i] == key)
            {
                return node.Entries[i];
            }

            return node.IsLeaf ? 0 : Search(node.Children[i], key);
        }

        private void SplitChild(Node parentNode, int nodeToBeSplitIndex, Node nodeToBeSplit)
        {
            // Create new node for bigger part 
            Node newNode = new Node(Degree);

            // Update parent with new key
            parentNode.Entries.Insert(nodeToBeSplitIndex, nodeToBeSplit.Entries[Degree - 1]);
            // Update with child
            parentNode.Children.Insert(nodeToBeSplitIndex + 1, newNode);

            // Get middle key of splitted node
            newNode.Entries.AddRange(nodeToBeSplit.Entries.GetRange(Degree, Degree - 1));

            // Remove it
            nodeToBeSplit.Entries.RemoveRange(Degree - 1, Degree);

            if (!nodeToBeSplit.IsLeaf)
            {
                // Move childs to new node
                newNode.Children.AddRange(nodeToBeSplit.Children.GetRange(Degree, Degree));
                nodeToBeSplit.Children.RemoveRange(Degree, Degree);
            }
        }

        private void InsertNonFull(Node node, int newKey)
        {
            // Find index to insert key
            int positionToInsert = node.Entries.TakeWhile(entry => newKey >= entry).Count();

            // Leaf node
            if (node.IsLeaf)
            {
                node.Entries.Insert(positionToInsert, newKey);
                return;
            }

            // Check if split required else move down to subtree
            Node child = node.Children[positionToInsert];
            if (child.HasReachedMaxEntries)
            {
                SplitChild(node, positionToInsert, child);
                if (newKey > node.Entries[positionToInsert])
                {
                    positionToInsert++;
                }
            }

            InsertNonFull(node.Children[positionToInsert], newKey);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            ToString(Root, 0, sb);
            return sb.ToString();
        }

        private void ToString(Node root, int offset, StringBuilder output)
        {
            if (root.IsLeaf)
            {
                output.AppendLine(new string(' ', offset) + root);
            }
            else
            {
                ToString(root.Children.Last(), offset + 4, output);
                for (int i = root.Entries.Count - 1; i >= 0; i--)
                {
                    output.AppendLine(new string(' ', offset) + root.Entries[i]);
                    ToString(root.Children[i], offset + 4, output);
                }
            }
        }

        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }
    }

}
