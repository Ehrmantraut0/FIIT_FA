using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Security.AccessControl;
using System.Security.Cryptography;
using TreeDataStructures.Implementations.BST;
using TreeDataStructures.Implementations.Treap;

namespace TreeDataStructures.Implementations.Splay;

public class SplayTree<TKey, TValue> : BinarySearchTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    protected override BstNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);

    public override void Add(TKey key, TValue value)
    {
        BstNode<TKey, TValue> newNode = CreateNode(key, value);
        OnNodeAdded(newNode);
    }

    protected override void OnNodeAdded(BstNode<TKey, TValue> newNode)
    {
        (BstNode<TKey, TValue>? left, BstNode<TKey, TValue>? right) = Split(newNode.Key);

        if (right != null && Comparer.Compare(newNode.Key, right.Key) == 0)
        {
            right.Value = newNode.Value;
            right.Left = left;
            left?.Parent = right;
        }
        else
        {
            Root = newNode;
            Count++;

            newNode.Right = right;
            right?.Parent = newNode;

            newNode.Left = left;
            left?.Parent = newNode;

        }

    }


    protected (BstNode<TKey, TValue>? left, BstNode<TKey, TValue>? right) Split(TKey key)

    {
        if (Root == null)
        {
            return (null, null);
        }

        BstNode<TKey, TValue>? lowerBound = FindLowerBound(key, out bool isFinded);
        BstNode<TKey, TValue>? left;
        BstNode<TKey, TValue>? right;

        if (isFinded)
        {
            right = Splay(lowerBound!);
            left = right.Left;
            right.Left = null;
            return (left, right);
        }
        else
        {
            left = Splay(lowerBound!);
            return (left, null);
        }
    }




    protected BstNode<TKey, TValue> Merge(BstNode<TKey, TValue>? t1, BstNode<TKey, TValue>? t2)
    {
        if (t1 == null)
        {

            return t2!;
        }
        else if (t2 == null)
        {

            return t1!;
        }
        else
        {
            t1 = Splay(FindMax(t1));
            t1.Right = t2;
            t2.Parent = t1;
            return t1;
        }
    }

    protected BstNode<TKey, TValue> FindMax(BstNode<TKey, TValue> root)
    {
        BstNode<TKey, TValue> max = root;
        while (root.Right != null)
        {
            root = root.Right;
            max = root;
        }

        return max;
    }


    protected BstNode<TKey, TValue> Splay(BstNode<TKey, TValue> x)
    {
        BstNode<TKey, TValue>? p = x.Parent;
        while (p != null)
        {
            if (p.Parent == null)
            {
                if (x.IsLeftChild)
                {
                    RotateRight(p);
                }
                else
                {
                    RotateLeft(p);
                }

            }
            else if (x.IsLeftChild && p.IsLeftChild)
            {
                RotateDoubleRight(p.Parent);
            }
            else if (x.IsRightChild && p.IsRightChild)
            {
                RotateDoubleLeft(p.Parent);
            }
            else if (x.IsRightChild && p.IsLeftChild)
            {
                RotateBigRight(p.Parent!);
            }
            else if (x.IsLeftChild && p.IsRightChild)
            {
                RotateBigLeft(p.Parent!);
            }

            p = x.Parent;
        }
        return x;
    }

    protected BstNode<TKey, TValue>? FindLowerBound(TKey key, out bool isFinded)
    {
        BstNode<TKey, TValue>? res = null;
        BstNode<TKey, TValue>? current = Root;
        isFinded = false;
        while (current != null)
        {
            int cmp = Comparer.Compare(key, current.Key);
            if (cmp < 0)
            {
                if (res == null)
                {
                    res = current;

                }
                else if (Comparer.Compare(current.Key, res.Key) < 0)
                {
                    res = current;
                }
                current = current.Left;
                isFinded = true;
            }
            else if (cmp > 0)
            {
                if (res == null && current.Right == null)
                {
                    res = current;
                    isFinded = false;
                }
                current = current.Right;
            }
            else
            {
                isFinded = true;
                return current;
            }
        }

        return res;
    }

    protected override void RemoveNode(BstNode<TKey, TValue> node)
    {
        OnNodeRemoved(null, node);
    }


    protected override BstNode<TKey, TValue>? FindNode(TKey key)
    {
        if (Root == null)
        {
            return null;
        }

        BstNode<TKey, TValue>? current = Root;
        BstNode<TKey, TValue>? prevNode = current;
        while (current != null)
        {
            int cmp = Comparer.Compare(key, current.Key);
            if (cmp == 0)
            {
                Splay(current);
                return current;
            }
            prevNode = current;
            current = cmp < 0 ? current.Left : current.Right;
        }
        Splay(prevNode);
        return null;
    }


    protected override void OnNodeRemoved(BstNode<TKey, TValue>? _, BstNode<TKey, TValue>? deletedNode)
    {
        deletedNode.Left?.Parent = null;
        deletedNode.Right?.Parent = null;
        Root = Merge(deletedNode.Left, deletedNode.Right);

    }


}