using Microsoft.VisualBasic;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using TreeDataStructures.Interfaces;

namespace TreeDataStructures.Core;

public abstract class BinarySearchTreeBase<TKey, TValue, TNode>(IComparer<TKey>? comparer = null) 
    : ITree<TKey, TValue>
    where TNode : Node<TKey, TValue, TNode>
{
    protected TNode? Root;
    public IComparer<TKey> Comparer { get; protected set; } = comparer ?? Comparer<TKey>.Default; // use it to compare Keys

    public int Count { get; protected set; }
    
    public bool IsReadOnly => false;

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>(Count);

            foreach (var node in this.InOrder())
            {
                keys.Add(node.Key);
            }

            return keys;
        }

    }
    
    public ICollection<TValue> Values {
        get
        {
            var Values = new List<TValue>(Count);

            foreach (var node in this.InOrder())
            {
                Values.Add(node.Value);
            }

            return Values;
        }
    }
    
    
    public virtual void Add(TKey key, TValue value)
    {
        //throw new NotImplementedException(
        //    "Implement standard BST add logic using <CreateNode(key, value)> and OnNodeAdded(newNode)");
        TNode newNode = this.CreateNode(key, value);
        if (this.Root == null)
        {
            this.Root = newNode;
            return;
        }

        TNode? temp = this.Root;
        TNode parent = this.Root;
        int cmp = 0;
        while (temp != null)
        {
            parent = temp;
            cmp = Comparer.Compare(key, temp.Key);
            temp = cmp <= 0 ? temp.Left : temp.Right;
        }

        if (cmp <= 0)
        {
            parent.Left = newNode;
        }
        else
        {
            parent.Right = newNode;
        }
        newNode.Parent = parent;
        this.Count++;
        this.OnNodeAdded(newNode);

    }

    
    public virtual bool Remove(TKey key)
    {
        TNode? node = FindNode(key);
        if (node == null) { return false; }

        RemoveNode(node);
        this.Count--;
        return true;
    }
    
    
    protected virtual void RemoveNode(TNode node)
    {
        //throw new NotImplementedException("Implement standard BST delete logic using Transplant helper");
        TNode? transplonted_node;
        if (node.Left == null)
        {
            this.Transplant(node, node.Right);
            transplonted_node = node.Right;
        }
        else if (node.Right == null)
        {
            this.Transplant(node, node.Left);
            transplonted_node = node.Left;
        }
        else
        {
            TNode minRight = node.Right;
            while (minRight.Left != null)
            {
                minRight = minRight.Left;
            }

            if (minRight.Parent != node)
            {
                this.Transplant(minRight, minRight.Right);
                minRight.Right = node.Right;
                node.Right.Parent = minRight;
            }

            this.Transplant(node, minRight);
            transplonted_node = minRight;

            minRight.Left = node.Left;
            minRight.Left.Parent = minRight;
        }

        this.OnNodeRemoved(transplonted_node?.Parent, transplonted_node);


    }

    public virtual bool ContainsKey(TKey key) => FindNode(key) != null;
    
    public virtual bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        TNode? node = FindNode(key);
        if (node != null)
        {
            value = node.Value;
            return true;
        }
        value = default;
        return false;
    }

    public TValue this[TKey key]
    {
        get => TryGetValue(key, out TValue? val) ? val : throw new KeyNotFoundException();
        set => Add(key, value);
    }

    
    #region Hooks
    
    /// <summary>
    /// Вызывается после успешной вставки
    /// </summary>
    /// <param name="newNode">Узел, который встал на место</param>
    protected virtual void OnNodeAdded(TNode newNode) { }
    
    /// <summary>
    /// Вызывается после удаления. 
    /// </summary>
    /// <param name="parent">Узел, чей ребенок изменился</param>
    /// <param name="child">Узел, который встал на место удаленного</param>
    protected virtual void OnNodeRemoved(TNode? parent, TNode? child) { }
    
    #endregion
    
    
    #region Helpers
    protected abstract TNode CreateNode(TKey key, TValue value);
    
    
    protected TNode? FindNode(TKey key)
    {
        TNode? current = Root;
        while (current != null)
        {
            int cmp = Comparer.Compare(key, current.Key);
            if (cmp == 0) { return current; }
            current = cmp < 0 ? current.Left : current.Right;
        }
        return null;
    }

    protected void RotateLeft(TNode x)
    {
        if (x.Right == null)
        {
            return;
        }

        TNode y = x.Right;

        Transplant(x, y);
        x.Parent = y;
        x.Right = y.Left;
        y.Left = x;
    }

    protected void RotateRight(TNode y)
    {
        if (y.Left == null)
        {
            return;
        }
         
        TNode x = y.Left;

        Transplant(y, x);
        y.Parent = x;
        y.Left = x.Right;
        x.Right = y;
    }
    
    protected void RotateBigLeft(TNode x)
    {
        this.RotateRight(x.Right!);

        this.RotateLeft(x);
    }
    
    protected void RotateBigRight(TNode y)
    {
        this.RotateLeft(y.Left!);
        this.RotateRight(y);
    }
    
    protected void RotateDoubleLeft(TNode x)
    {
        this.RotateLeft(x);
        this.RotateLeft(x.Parent!);
    }
    
    protected void RotateDoubleRight(TNode y)
    {
        this.RotateRight(y);
        this.RotateRight(y.Parent!);
    }
    
    protected void Transplant(TNode u, TNode? v)
    {
        if (u.Parent == null)
        {
            Root = v;
        }
        else if (u.IsLeftChild)
        {
            u.Parent.Left = v;
        }
        else
        {
            u.Parent.Right = v;
        }
        v?.Parent = u.Parent;
    }
    #endregion
    
    public IEnumerable<TreeEntry<TKey, TValue>>  InOrder() => InOrderTraversal(Root);
    
    private IEnumerable<TreeEntry<TKey, TValue>>  InOrderTraversal(TNode? node)
    {
        if (node == null) {  yield break; }
        throw new NotImplementedException();
    }
    
    public IEnumerable<TreeEntry<TKey, TValue>>  PreOrder() => new TreeIterator(TraversalStrategy.PreOrder);
    public IEnumerable<TreeEntry<TKey, TValue>>  PostOrder() => new TreeIterator(TraversalStrategy.PostOrder);
    public IEnumerable<TreeEntry<TKey, TValue>>  InOrderReverse() => new TreeIterator(TraversalStrategy.InOrderReverse);
    public IEnumerable<TreeEntry<TKey, TValue>>  PreOrderReverse() => new TreeIterator(TraversalStrategy.PreOrderReverse);
    public IEnumerable<TreeEntry<TKey, TValue>>  PostOrderReverse() => new TreeIterator(TraversalStrategy.PostOrderReverse);
    
    /// <summary>
    /// Внутренний класс-итератор. 
    /// Реализует паттерн Iterator вручную, без yield return (ban).
    /// </summary>
    private struct TreeIterator : 
        IEnumerable<TreeEntry<TKey, TValue>>,
        IEnumerator<TreeEntry<TKey, TValue>>
    {
        public TreeIterator(TraversalStrategy strategy)
        {
            this._strategy = strategy;
        }

        // probably add something here
        private TreeEntry<TKey, TValue> _current;
        private TraversalStrategy _strategy;
        
        public IEnumerator<TreeEntry<TKey, TValue>> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
        
        public TreeEntry<TKey, TValue> Current => this._current;
        object IEnumerator.Current => Current;
        
        
        public bool MoveNext()
        {
            if (_strategy == TraversalStrategy.InOrder)
            {
                throw new NotImplementedException();
            }
            throw new NotImplementedException("Strategy not implemented");
        }
        
        public void Reset()
        {
            throw new NotImplementedException();
        }

        
        public void Dispose()
        {
            // TODO release managed resources here
        }
    }
    
    
    private enum TraversalStrategy { InOrder, PreOrder, PostOrder, InOrderReverse, PreOrderReverse, PostOrderReverse }
    
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        throw new NotImplementedException();
    }
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
    public void Clear() { Root = null; Count = 0; }
    public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new NotImplementedException();
    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
}