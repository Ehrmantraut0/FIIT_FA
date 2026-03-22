using System.Runtime.CompilerServices;
using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.RedBlackTree;

public class RedBlackTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, RbNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{

    protected override RbNode<TKey, TValue> CreateNode(TKey key, TValue value)
    {
        return new RbNode<TKey, TValue>(key, value);
    }
    
    protected override void OnNodeAdded(RbNode<TKey, TValue> newNode)
    {
        BalanceTree(newNode);
    }

    protected void  BalanceTree(RbNode<TKey, TValue> node)
    {
        if (node == Root)
        {
            node.Color = RbColor.Black;
            return;
        }

        if (node.Parent!.Color == RbColor.Red)
        {
            if (node.Uncle != null && IsRed(node.Uncle))
            {
                InsertCaseRedUncle(node);
            }
            else
            {
                InsertCaseBlackUncle(node);
            }
        }
        
    }

    protected void InsertCaseRedUncle(RbNode<TKey, TValue> node)
    {
        node.Uncle!.Color = RbColor.Black;
        node.Parent!.Color = RbColor.Black;
        node.GParent!.Color = RbColor.Red;

        BalanceTree(node.GParent);
    }


    protected void InsertCaseBlackUncle(RbNode<TKey, TValue> node)
    {
         if (node.IsRightChild && node.Parent!.IsLeftChild)
        {
            RotateLeft(node.Parent);
            node = node.Left!;
        }
        else if (node.IsLeftChild && node.Parent!.IsRightChild)
        {
            RotateRight(node.Parent);
            node = node.Right!;
        }

        InsertCaseLLRR(node);
    }

    protected void InsertCaseLLRR(RbNode<TKey, TValue> node)
    {

        node.Parent!.Color = RbColor.Black;
        node.GParent!.Color = RbColor.Red;
        if (node.IsLeftChild && node.Parent.IsLeftChild)
        {
            RotateRight(node.GParent);
        }
        else
        {
            RotateLeft(node.GParent);
        }
    }

    protected override void OnNodeRemoved(RbNode<TKey, TValue>? parent, RbNode<TKey, TValue>? child)
    {
        if (IsRed(parent))
        {
            return;
        }
        else if (child != null)
        {
            child.Color = RbColor.Black;
            return;
        }
        DeleteCase1(child, parent.Parent);
    }

    protected void DeleteCase1(RbNode<TKey, TValue>? node, RbNode<TKey, TValue>? parent)
    {
        if (parent != null)
        {
            DeleteCase2(node, parent);
        }
    }

    protected void DeleteCase2(RbNode<TKey, TValue>? node, RbNode<TKey, TValue> parent)
    {
        RbNode<TKey, TValue> brother = Brother(node, parent)!;

        if (IsRed(brother))
        {
            
            parent.Color = RbColor.Red;
            brother.Color = RbColor.Black;
            if (brother.IsLeftChild)
            {
                RotateRight(parent);
            }
            else
            {
                RotateLeft(parent);
            }
        }
        DeleteCase3(node, parent);
    }

    protected void DeleteCase3(RbNode<TKey, TValue>? node, RbNode<TKey, TValue> parent)
    {
        RbNode<TKey, TValue> brother = Brother(node, parent)!;
        
        if (IsBlack(parent) &&
            IsBlack(brother) &&
            IsBlack(brother.Left) &&
            IsBlack(brother.Right))
        {
            brother.Color = RbColor.Red;
            DeleteCase1(parent, parent.Parent);
        }
        else
        {
            DeleteCase4(node, parent);
        }



    }

    protected void DeleteCase4(RbNode<TKey, TValue>? node, RbNode<TKey, TValue> parent)
    {
        RbNode<TKey, TValue> brother = Brother(node, parent)!;

        if (IsRed(parent) &&
            IsBlack(brother) &&
            IsBlack(brother.Left) &&
            IsBlack(brother.Right))
        {
            parent.Color = RbColor.Black;
            brother.Color = RbColor.Red;
        }
        else
        {
            DeleteCase5(node, parent);
        }


    }

    protected void DeleteCase5(RbNode<TKey, TValue>? node, RbNode<TKey, TValue> parent)
    {
        RbNode<TKey, TValue> brother = Brother(node, parent)!;

        if (IsBlack(brother))
        {
            if (IsRed(brother.Left) && IsBlack(brother.Right) && brother.IsRightChild)
            {
                brother.Color = RbColor.Red;
                brother.Left!.Color = RbColor.Black;
                RotateRight(brother);
            }
            else if (IsRed(brother.Right) && IsBlack(brother.Left) && brother.IsLeftChild) 
            {
                brother.Color = RbColor.Red;
                brother.Right!.Color = RbColor.Black;
                RotateLeft(brother);
            }
        }
        DeleteCase6(node, parent);

    }

    protected void DeleteCase6(RbNode<TKey, TValue>? node, RbNode<TKey, TValue> parent)
    {
        RbNode<TKey, TValue> brother = Brother(node, parent)!;

        brother.Color = parent.Color;
        parent.Color = RbColor.Black;

        if (IsBlack(brother) && brother.IsRightChild && IsRed(brother.Right))
        {
            brother.Right!.Color = RbColor.Black;
            RotateLeft(parent);
        }
        else if (IsBlack(brother) && brother.IsLeftChild && IsRed(brother.Left))
        {
            brother.Left!.Color = RbColor.Black;
            RotateRight(parent);
        }

    }


    protected bool IsBlack(RbNode<TKey, TValue>? node)
    {
        if (node != null)
        {
            return node.Color == RbColor.Black;
        }
        else
        {
            return true;
        }
    }

    protected bool IsRed(RbNode<TKey, TValue>? node)
    {
        if (node != null)
        {
            return node.Color == RbColor.Red;
        }
        else
        {
            return false;
        }
    }

         

    protected RbNode<TKey, TValue>? Brother(RbNode<TKey, TValue>? node, RbNode<TKey, TValue>? parent)
    {
        if (parent == null)
        {
            return null;
        }

        return parent.Left == node ? parent.Right: parent.Left;
    }

}