using System.Net;
using System.Net.NetworkInformation;
using System.Security;
using TreeDataStructures.Core;
using TreeDataStructures.Implementations.BST;

namespace TreeDataStructures.Implementations.AVL;

public class AvlTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, AvlNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    protected override AvlNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);

    protected override void OnNodeRemoved(AvlNode<TKey, TValue>? parent, AvlNode<TKey, TValue>? child)
    {
        if (child != null)
        {
            OnNodeAdded(child);
        }
        else
        {
            OnNodeAdded(parent!.Parent!);
        }
    }


    protected override void OnNodeAdded(AvlNode<TKey, TValue> current)
    {
        AvlNode<TKey, TValue>? parent = current.Parent!;
        while (UpdateHeight(current))
        {
            BalanceTree(current);
            if (parent == null)
            {
                break;
            }
            else
            {
                current = parent;
                parent = current.Parent;
            }
        }

    }


    private void BalanceTree(AvlNode<TKey, TValue> node)
    {
        AvlNode<TKey, TValue>? parent = node.Parent;
        int balance = CheckBalance(node);
        if (balance == 0 || balance == 1 || balance == -1)
        {
            return;
        }
        else if (balance == 2)
        {
            if (GetHeight(node?.Left?.Left) > GetHeight(node?.Left?.Right))
            {
                RotateRight(node!);
            }
            else if  (GetHeight(node?.Left?.Left) < GetHeight(node?.Left?.Right))
            {
                RotateBigRight(node!);
            }

        }
        else if (balance == -2)
        {
            if (GetHeight(node?.Right?.Right) > GetHeight(node?.Right?.Left))
            {
                RotateLeft(node!);
            }
            else if (GetHeight(node?.Right?.Right) < GetHeight(node?.Right?.Left))
            {
                RotateBigLeft(node!);
            }
        }

        node = parent ?? Root!;

        UpdateHeight(node.Left);
        UpdateHeight(node.Right);
        UpdateHeight(node);
    }

    private int GetHeight(AvlNode<TKey, TValue>? node)
    {
        return node?.Height ?? 0;
    }

    private int CheckBalance(AvlNode<TKey, TValue> node) 
    {
        int left_height = GetHeight(node.Left);
        int right_height = GetHeight(node.Right);

        return left_height - right_height;
    }

    private bool UpdateHeight(AvlNode<TKey, TValue>? node)
    {
        if (node == null)
        {
            return false;
        }

        int old_height = node.Height;
        int left_height = GetHeight(node.Left);
        int right_height = GetHeight(node.Right);

        node.Height = Math.Max(left_height, right_height);

        return old_height != node.Height;
    }

    
}