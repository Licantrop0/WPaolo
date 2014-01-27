using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SgarbiMix.WP7.Model
{
    /// <summary>
    /// A class used to expose the Key property on a dynamically-created Linq grouping.
    /// The grouping will be generated as an internal class, so the Key property will not
    /// otherwise be available to databind.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TElement">The type of the items.</typeparam>
    public sealed class LLSGroup<TKey, TElement> : IGrouping<TKey, TElement>, IList<TElement>
    {
        public TKey Key { get; set; }
        public bool HasElements { get { return _elements.Any(); } }

        private readonly IList<TElement> _elements;

        public LLSGroup(IGrouping<TKey, TElement> internalGrouping)
        {
            Key = internalGrouping.Key;
            _elements = internalGrouping.ToList();
        }

        public LLSGroup(TKey key)
        {
            //creates an empty group
            Key = key;
            _elements = new List<TElement>();
        }

        public LLSGroup(TKey key, IEnumerable<TElement> elements)
        {
            Key = key;
            _elements = elements.ToList();
        }

        public override bool Equals(object obj)
        {
            var that = obj as LLSGroup<TKey, TElement>;
            return (that != null) && (this.Key.Equals(that.Key));
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        #region IList Members
        
        public int IndexOf(TElement item)
        {
            return _elements.IndexOf(item);
        }

        public void Insert(int index, TElement item)
        {
            _elements.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _elements.RemoveAt(index);
        }

        public TElement this[int index]
        {
            get
            {
                return _elements[index];
            }
            set
            {
                _elements[index] = value;
            }
        }

        public void Add(TElement item)
        {
            _elements.Add(item);
        }

        public void Clear()
        {
            _elements.Clear();
        }

        public bool Contains(TElement item)
        {
            return _elements.Contains(item);
        }

        public void CopyTo(TElement[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _elements.Count; }
        }

        public bool IsReadOnly
        {
            get { return _elements.IsReadOnly; }
        }

        public bool Remove(TElement item)
        {
            return _elements.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        #endregion
    }
}
