using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMaxTree
{
    public class GTree<T>
    {
    # region private members

        private Node<T> _root;

        private List<List<Node<T>>> _layers;

    # endregion

    # region constructors

        public GTree(T value)
        {
            _root = new Node<T>(value, 1, this);

            _layers = new List<List<Node<T>>>();
            _layers.Add(new List<Node<T>>());
            _layers[0].Add(_root);
        }

    # endregion

    # region properties

        public Node<T> Root 
        {
            get { return _root; }
        }

        public List<List<Node<T>>> Layers { get { return _layers; } }

    # endregion
    }
}
