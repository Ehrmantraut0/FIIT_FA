using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
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
        }
        else
        {
            TNode? temp = this.Root;
            TNode parent = this.Root;
            int cmp = 0;
            while (temp != null)
            {
                parent = temp;
                cmp = Comparer.Compare(key, temp.Key);
                if (cmp == 0)
                {
                    parent.Value = value;
                    return;
                }
                temp = cmp < 0 ? temp.Left : temp.Right;
            }

            if (cmp < 0)
            {
                parent.Left = newNode;
            }
            else 
            {
                parent.Right = newNode;
            }
           newNode.Parent = parent;
        }

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
        if (Count == 1)
        {
            Root = null;
            return;
        }

        TNode balancingStart;
        if (node.Left == null && node.Right != null)
        {
            this.Transplant(node, node.Right);
            balancingStart = node.Right;
        }
        else if (node.Right == null && node.Left != null)
        {
            this.Transplant(node, node.Left);
            balancingStart = node.Left;
        }
        else if (node.Left == null && node.Right == null)
        {
            balancingStart = node.Parent!;
            this.Transplant(node, null);
        }
        else
        {
            TNode minRight = node.Right!;
            while (minRight.Left != null)
            {
                minRight = minRight.Left;
            }

            if (minRight.Parent != node)
            {

                this.Transplant(minRight, minRight.Right);
                minRight.Right = node.Right;
                node.Right!.Parent = minRight;
            }

            balancingStart = minRight.Parent!;
            this.Transplant(node, minRight);

            minRight.Left = node.Left;
            minRight.Left!.Parent = minRight;
        }

        this.OnNodeRemoved(balancingStart, null);


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
    
    public IEnumerable<TreeEntry<TKey, TValue>>  InOrder() => new TreeIterator(TraversalStrategy.InOrder, Root);
    //private IEnumerable<TreeEntry<TKey, TValue>> InOrderTraversal(TNode? node)
    //     => new TreeIterator(TraversalStrategy.InOrder, node);

    public IEnumerable<TreeEntry<TKey, TValue>>  PreOrder() => new TreeIterator(TraversalStrategy.PreOrder, Root);
    public IEnumerable<TreeEntry<TKey, TValue>>  PostOrder() => new TreeIterator(TraversalStrategy.PostOrder, Root);
    public IEnumerable<TreeEntry<TKey, TValue>>  InOrderReverse() => new TreeIterator(TraversalStrategy.InOrderReverse, Root);
    public IEnumerable<TreeEntry<TKey, TValue>>  PreOrderReverse() => new TreeIterator(TraversalStrategy.PreOrderReverse, Root);
    public IEnumerable<TreeEntry<TKey, TValue>>  PostOrderReverse() => new TreeIterator(TraversalStrategy.PostOrderReverse, Root);
    
    /// <summary>
    /// Внутренний класс-итератор. 
    /// Реализует паттерн Iterator вручную, без yield return (ban).
    /// </summary>
    private struct TreeIterator: 
        IEnumerable<TreeEntry<TKey, TValue>>,
        IEnumerator<TreeEntry<TKey, TValue>>
    {
        public TreeIterator(TraversalStrategy strategy, TNode? root)
        {
            this._strategy = strategy;
            this._root = root;
            this._current = null;
            this._depth = 0;
            this._prevNode = null;
        }

        // probably add something here
        private TNode ?_current;
        private int _depth;
        private TraversalStrategy _strategy;
        private TNode ?_prevNode;
        private TNode ?_root;


        public IEnumerator<TreeEntry<TKey, TValue>> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        public TreeEntry<TKey, TValue> Current 
        {
            get
            {
                if (_current == null)
                    throw new InvalidOperationException("Enumerator is not started or has finished");

                return new TreeEntry<TKey, TValue>(_current.Key, _current.Value, _depth);
            }
        }
        object IEnumerator.Current => Current;
        
        
        public bool MoveNext()
        {
            if (this._root == null)
            {
                return false;
            }
            bool res = true;

            switch (_strategy)
            {
                case (TraversalStrategy.InOrder):
                    res = MoveInOrder();
                    break;
                case (TraversalStrategy.PreOrder):
                    res = MovePreOrder();
                    break;
                case (TraversalStrategy.PostOrder):
                    res = MovePostOrder();
                    break;
                case (TraversalStrategy.InOrderReverse):
                    res = MoveInOrderReverse();
                    break;
                case (TraversalStrategy.PreOrderReverse):
                    res = MovePreOrderReverse();
                    break;
                case (TraversalStrategy.PostOrderReverse):
                    res = MovePostOrderReverse();
                    break;
            }

            return res;
        
        }

        public bool MovePostOrderReverse()
        {
            if (_current == null)
            {
                _current = this._root;
                _depth++;
                return true;
            }

            while ((_current.Left == null && _current.Right == null) || ((_current.Right == _prevNode) && (_current.Left == null)) || (_current.Left == _prevNode))
            {
                if (_current.Parent == null)
                {
                    _current = null;
                    return false;
                }
                _depth--;
                _prevNode = _current;
                _current = _current.Parent;
            }

            if ((_current.Right != null && _current.Parent == _prevNode) || _prevNode == null)
            {
               _prevNode = _current;
                _depth++;
                _current = _current.Right;
            }
            else
            {
                _prevNode = _current;
                _depth++;
                _current = _current.Left;
            }

            return true;
        }

        public bool MovePreOrderReverse()
        {
            if (_current == null)
            {
                _depth++;
                _current = this._root!;
            }


            if (_prevNode != null)
            {
                if (_current.Parent == null)
                {
                    _current = null;
                    return false;
                }
                _depth--;
                _prevNode = _current;
                _current = _current.Parent!;
                if (_current.Left == _prevNode || (_current.Left == null && _current.Right == _prevNode))
                {
                    return true;
                }
            }



            while (!(_current.Left == null && _current.Right == null))
            {
                while ((_current.Parent == _prevNode || _prevNode == null) && _current.Right != null)
                {
                    _depth++;
                    _prevNode = _current;
                    _current = _current.Right;
                }

                while (_current.Left != null && (_current.Right == null || _prevNode == _current.Right))
                {
                    _depth++;
                    _prevNode = _current;
                    _current = _current.Left;
                }
            }

            return true;
        }

        public bool MoveInOrderReverse()
        {

            if (_current == null)
            {
                _depth++;
                _current = this._root!;
                if (_current.Right == null)
                {
                    return true;
                }
            }



            while ((_current.Left == null && _current.Right == null) || (_current.Right == _prevNode && _current.Left == null) || (_current.Left == _prevNode))
            {
                if (_current.Parent == null)
                {
                    _current = null;
                    return false;
                }
                _depth--;
                _prevNode = _current;
                _current = _current.Parent!;
                if (_prevNode == _current.Right)
                {
                    return true;
                }
            }


            if (_current.Left != null && (_current.Right == null || _prevNode == _current.Right))
            {
                _depth++;
                _prevNode = _current;
                _current = _current.Left;
            }

            while ((_current.Parent == _prevNode || _prevNode == null) && _current.Right != null)
            {
                _depth++;
                _prevNode = _current;
                _current = _current.Right;
            }

            return true;
        }


        public bool MovePostOrder()
        {
            if (_current == null)
            {
                _depth++;
                _current = this._root!;
            }


            if (_prevNode != null)
            {
                if (_current.Parent == null)
                {
                    _current = null;
                    return false;
                }
                _depth--;
                _prevNode = _current;
                _current = _current.Parent!;
                if (_current.Right == _prevNode || (_current.Right == null && _current.Left == _prevNode))
                { 
                    return true;
                }
            }



            while (!(_current.Left == null && _current.Right == null))
            {
                while ((_current.Parent == _prevNode || _prevNode == null) && _current.Left != null)
                {
                    _depth++;
                    _prevNode = _current;
                    _current = _current.Left;
                }

                while (_current.Right != null && (_current.Left == null || _prevNode == _current.Left))
                {
                    _depth++;
                    _prevNode = _current;
                    _current = _current.Right;
                }

            }

            return true;
        }

        public bool MoveInOrder()
        {
            if (_current == null)
            {
                _depth++;
                _current = this._root!;
                if (_current.Left == null)
                {
                    return true;
                }
            }



            while ((_current.Left == null && _current.Right == null) || (_current.Left == _prevNode && _current.Right == null) || (_current.Right == _prevNode))
            {
                if (_current.Parent == null && _prevNode != _current)
                {
                    _current = null;
                    return false;
                }
                _depth--;
                _prevNode = _current;
                _current = _current.Parent!;
                if (_prevNode == _current.Left)
                {
                    return true;
                }
            }


            if (_current.Right != null && (_current.Left == null || _prevNode == _current.Left))
            {
                _depth++;
                _prevNode = _current;
                _current = _current.Right;
            }

            while ((_current.Parent == _prevNode || _prevNode == null) && _current.Left != null)
            {
                _depth++;
                _prevNode = _current;
                _current = _current.Left;
            }
            return true;
        }

        public bool MovePreOrder()
        {
            if (_current == null)
            {
                _current = this._root;
                _depth++;
                return true;
            }

            while ((_current.Left == null && _current.Right == null) || ((_current.Left == _prevNode) && (_current.Right == null)) || (_current.Right == _prevNode))
            {
                if (_current.Parent == null)
                {
                    _current = null;
                    return false;
                }
                _depth--;
                _prevNode = _current;
                _current = _current.Parent;
            }

            if ((_current.Left != null && _current.Parent == _prevNode) || _prevNode == null)
            {
                _prevNode = _current;
                _current = _current.Left;
                _depth++;
            }
            else
            {
                _prevNode = _current;
                _current = _current.Right;
                _depth++;
            }

            return true;

        }

        public void Reset()
        {
            if (_root != null)
            {
                _current = null;
                _prevNode = null;
                _depth = 0;
            }
        }

        
        public void Dispose()
        {
            // TODO release managed resources here
        }
    }
    
    
    private enum TraversalStrategy { InOrder, PreOrder, PostOrder, InOrderReverse, PreOrderReverse, PostOrderReverse }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => InOrder().Select(e => new KeyValuePair<TKey, TValue>(e.Key, e.Value)).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
    public void Clear() { Root = null; Count = 0; }
    public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array.Length - arrayIndex < Count)
        {
            throw new ArgumentException("Not enough capacity");
        }

        foreach (TreeEntry<TKey, TValue> item in InOrder())
        {
            array[arrayIndex++] = new KeyValuePair<TKey, TValue>(item.Key, item.Value);
        }
    }
    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
}