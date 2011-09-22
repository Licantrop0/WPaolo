using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMaxTree
{
    public class Node<T>
    {
    # region private members

        private GTree<T> _gTree;

        private List<Node<T>> _children;

        private int _depth;

        private T _content;

    # endregion

    # region constructors

        public Node (T value, int depth, GTree<T> gTree)
        {
            _content = value;
            _depth = depth;
            _gTree = gTree;

            _children = new List<Node<T>>();
        }

    # endregion

    # region properties

        public int Depth {get{return _depth;} set{_depth = value;}}

        public T Content { get { return _content; } set { _content = value; } }

        public List<Node<T>> Children { get { return _children; } }

    # endregion

    # region public methods

        public void AddChild(T value)
        {
            int newNodeDepth = _depth + 1;

            Node<T> newNode = new Node<T>(value, newNodeDepth, _gTree);

            _children.Add(newNode);

            if (newNodeDepth > _gTree.Layers.Count)
            {
                _gTree.Layers.Add(new List<Node<T>>());
            }

            _gTree.Layers[newNodeDepth - 1].Add(newNode);
        }

    #endregion
    }
}
