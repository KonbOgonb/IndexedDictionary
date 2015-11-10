using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexedDictionary
{
    /// <summary>
    /// This class allows to add custom indices to Dictionary. They work as indices in SQL and allows one to perform faster search by index values.
    /// Usage of indices slows down insert/delete operation. You can turn indices off to perform lots of inserts/removalse and then rebuilt indices.
    /// </summary>
    /// <typeparam name="TKey">Dictionary key</typeparam>
    /// <typeparam name="TValue">Dictionary value</typeparam>
    public class IndexedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        #region indices
        protected class IndexHelper
        {
            public Func<TKey, TValue, object> IndexSelector;
            public Dictionary<object, HashSet<TKey>> IndexDict;
        }
        protected List<IndexHelper> indices = new List<IndexHelper>();
        bool useIndices = true;
        /// <summary>
        /// Turning on initiates indices rebuilding.
        /// </summary>
        public bool UseIndices
        {
            get
            {
                return useIndices;
            }
            set
            {
                useIndices = value;
                if (value)
                {
                    Rebuiltindices();
                }
            }
        }

        protected void Rebuiltindices()
        {
            foreach (var pair in innerDict)
            {
                AddToIndices(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Provide projection from TKey and TValue to desired index
        /// </summary>
        /// <param name="indexSelector">provide projection from TKey,TValue to index</param>
        protected IndexHelper CreateIndex(Func<TKey, TValue, object> indexSelector)
        {
            var index = new IndexHelper { IndexSelector = indexSelector, IndexDict = new Dictionary<object, HashSet<TKey>>() };
            indices.Add(index);
            return index;
        }

        private void AddToIndices(TKey key, TValue value)
        {
            if (!UseIndices)
            {
                return;
            }

            foreach (var index in indices)
            {
                var indexValue = index.IndexSelector(key, value);
                if (!index.IndexDict.ContainsKey(indexValue))
                {
                    index.IndexDict[indexValue] = new HashSet<TKey>();
                }
                index.IndexDict[indexValue].Add(key);
            }
        }

        private void RemoveFromIndices(TKey key, TValue value)
        {
            if (!UseIndices)
            {
                return;
            }
            foreach (var index in indices)
            {
                var indexValue = index.IndexSelector(key, value);
                index.IndexDict[indexValue].Remove(key);
            }
        }

        private void ClearIndices()
        {
            foreach (var index in indices)
            {
                index.IndexDict.Clear();
            }
        }
        #endregion
        #region IDict
        //primary internal collection
        Dictionary<TKey, TValue> innerDict { get; set; } = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            get
            {
                return innerDict[key];
            }

            set
            {
                innerDict[key] = value;
                AddToIndices(key, value);                
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return innerDict.Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return innerDict.Values;
            }
        }

        public int Count
        {
            get
            {
                return innerDict.Count;
            }
        }


        public void Add(TKey key, TValue value)
        {
            this[key] = value;
        }

        public void Clear()
        {
            innerDict.Clear();
            ClearIndices();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return innerDict.Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return innerDict.ContainsKey(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool Remove(TKey key)
        {
            TValue value;
            bool result = false;
            if (innerDict.TryGetValue(key, out value))
            {
                result = innerDict.Remove(key);
                if (result)
                {
                    RemoveFromIndices(key, value);
                }
            }
            return result;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return innerDict.TryGetValue(key, out value);
        }
        #endregion
        #region ICollection
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return (innerDict as ICollection<TKey>).IsReadOnly;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this[item.Key] = item.Value;
        }
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            (innerDict as ICollection).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return innerDict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
