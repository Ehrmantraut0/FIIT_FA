using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.Swift;
using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.Treap;

public class Treap<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, TreapNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    /// <summary>
    /// Разрезает дерево с корнем <paramref name="root"/> на два поддерева:
    /// Left: все ключи <= <paramref name="key"/>
    /// Right: все ключи > <paramref name="key"/>
    /// </summary>
    protected virtual (TreapNode<TKey, TValue>? Left, TreapNode<TKey, TValue>? Right) Split(
        TreapNode<TKey, TValue>? root,
        TKey key,
        ref TreapNode<TKey, TValue>? duplicate)
    {
        if (root == null)
        {
            return (null, null);
        }

        int cmp = Comparer.Compare(root.Key, key);
        if (cmp < 0)
        {
            (TreapNode<TKey, TValue>? left, TreapNode<TKey, TValue>? right) = Split(root.Right, key, ref duplicate);

            root.Right = left;
            left?.Parent = root;

            return (root, right);
        }
        else
        {
            if (cmp == 0)
            {
                duplicate = root;
            }

            (TreapNode<TKey, TValue>? left, TreapNode<TKey, TValue>? right) = Split(root.Left, key, ref duplicate);
            root.Left = right;

            right?.Parent = root;

            return (left, root);
        }
    }

    /// <summary>
    /// Сливает два дерева в одно.
    /// Важное условие: все ключи в <paramref name="left"/> должны быть меньше ключей в <paramref name="right"/>.
    /// Слияние происходит на основе Priority (куча).
    /// </summary>
    protected virtual TreapNode<TKey, TValue>? Merge(TreapNode<TKey, TValue>? left, TreapNode<TKey, TValue>? right)
    {
        if (left == null)
        {
            right?.Parent = null;
            return right;
        }
        if (right == null)
        {
            left.Parent = null;
            return left;
        }

        if (left.Priority > right.Priority)
        {
            left.Right = Merge(left.Right, right);
            if (left.Right != null)
                left.Right.Parent = left;
            left.Parent = null;
            return left;
        }
        else
        {
            right.Left = Merge(left, right.Left);
            right.Left?.Parent = right;
            right.Parent = null;
            return right;
        }
    }

    public override void Add(TKey key, TValue value)
    {
        TreapNode<TKey, TValue>? duplicate = null;
        (TreapNode<TKey, TValue>? left, TreapNode<TKey, TValue>? right) = Split(Root, key, ref duplicate);

        if (duplicate != null)
        {
            duplicate.Value = value;
            Root = Merge(left, right);
        }
        else
        {
            TreapNode<TKey, TValue> newNode = CreateNode(key, value);
            Root = Merge(Merge(left, newNode), right);
            Count++;
        }
    }

    public override bool Remove(TKey key)
    {
        TreapNode<TKey, TValue>? delNode = null;
        (TreapNode<TKey, TValue>? left, TreapNode<TKey, TValue>? rest) = Split(Root, key, ref delNode);

        if (delNode == null)
        {
            Root = Merge(left, rest);
            return false;
        }
        else
        {
            TKey sep = key;

            if (delNode.Right == null)
            {
                if (delNode.Parent == null)
                {
                    Root = left;
                    Count--;
                    return true;
                }

                sep = delNode.Parent.Key;
            }
            else
            {
                TreapNode<TKey, TValue> curr = delNode.Right;

                while (curr.Left != null)
                {
                    curr = curr.Left;
                }
                sep = curr.Key;
            }

            (TreapNode<TKey, TValue>? del, TreapNode<TKey, TValue>? right) = Split(rest, sep, ref delNode);
            Root = Merge(left, right);
            Count--;
            return true;
        }
    }

    protected override TreapNode<TKey, TValue> CreateNode(TKey key, TValue value)
    {
        return new TreapNode<TKey, TValue>(key, value);
    }

    protected override void OnNodeAdded(TreapNode<TKey, TValue> newNode)
    {
        return;
    }

    protected override void OnNodeRemoved(TreapNode<TKey, TValue>? parent, TreapNode<TKey, TValue>? child)
    {
        return;
    }
}