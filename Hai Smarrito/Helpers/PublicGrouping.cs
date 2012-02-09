using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace NientePanico.Helpers
{
    /// <summary>
    /// A class used to expose the Key property on a dynamically-created Linq grouping.
    /// The grouping will be generated as an internal class, so the Key property will not
    /// otherwise be available to databind.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TElement">The type of the items.</typeparam>
    public class LLSGroup<T> : IGrouping<char, T>, IEnumerable<T>
    {
        public char Key { get; set; }
        public bool HasElements { get { return _elements.Any(); } }

        private readonly IEnumerable<T> _elements;

        public LLSGroup(IGrouping<char, T> internalGrouping)
        {
            Key = internalGrouping.Key;
            _elements = internalGrouping;
        }

        public LLSGroup(char key)
        {
            //creates an empty group
            Key = key;
            _elements = Enumerable.Empty<T>();
        }

        public override bool Equals(object obj)
        {
            var that = obj as LLSGroup<T>;

            return (that != null) && (this.Key.Equals(that.Key));
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }


        #region IEnumerable Members
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        #endregion
    }
}
