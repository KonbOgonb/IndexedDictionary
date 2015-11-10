using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexedDictionary
{
    /// <summary>
    /// Collection for the solution of the task. Example of how indexedDictionary could be used
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TName"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class CompositeKeyDictionary<TId, TName, TValue> : IndexedDictionary<Tuple<TId, TName>, TValue>
        where TId : IEquatable<TId>
        where TName : IEquatable<TName>
    {
        private readonly IndexHelper idIndex;
        private readonly IndexHelper nameIndex;
        //Adding indices for Name and Id faster access
        public CompositeKeyDictionary()
        {
            idIndex = CreateIndex((Tuple<TId, TName> key, TValue val) => key.Item1);
            nameIndex = CreateIndex((Tuple<TId, TName> key, TValue val) => key.Item2);
        }

        public IEnumerable<TValue> this[TId id]
        {
            get
            {
                return GetByIndex(idIndex, id);
            }
        }

        public IEnumerable<TValue> this[TName name]
        {
            get
            {
                return GetByIndex(nameIndex, name);
            }
        }

        IEnumerable<TValue> GetByIndex<TIndex>(IndexHelper indexHelper, TIndex value)
        {
            lock (SyncRoot)
            {
                if (UseIndices)
                {
                    // fast solution to get data by single key element
                    return indexHelper.IndexDict[value].Select(key => this[key]);
                }
                // slow solution in case we disable indiced
                return innerDict.Keys.Where(key => key.Item2.Equals(value)).Select(key => innerDict[key]);
            }
        }
    }
}
