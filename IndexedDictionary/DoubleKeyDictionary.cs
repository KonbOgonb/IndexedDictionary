using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexedDictionary
{
    /// <summary>
    /// Just a sample, haw indexedDictionary could be used
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TName"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DoubleKeyDictionary<TId, TName, TValue> : IndexedDictionary<Tuple<TId, TName>, TValue>
        where TId : IEquatable<TId>
        where TName : IEquatable<TName>
    {
        private readonly IndexHelper idIndex;
        private readonly IndexHelper nameIndex;
        //Adding indices for Name and Id faster access
        public DoubleKeyDictionary()
        {
            idIndex = AddIndex((Tuple<TId, TName> key, TValue val) => key.Item1);
            nameIndex = AddIndex((Tuple<TId, TName> key, TValue val) => key.Item2);
        }

        public IEnumerable<TValue> this[TId id]
        {
            get
            {
                if (Useindices)
                {
                    // fast solution to get data by single key element
                    return idIndex.IndexDict[id].Select(key => this[key]);
                }
                return Keys.Where(key => key.Item1.Equals(id)).Select(key => this[key]);
            }
        }

        public IEnumerable<TValue> this[TName TName]
        {
            get
            {
                if (Useindices)
                {
                    // fast solution to get data by single key element
                    return nameIndex.IndexDict[TName].Select(key => this[key]);
                }
                return Keys.Where(key => key.Item2.Equals(TName)).Select(key => this[key]);
            }
        }
    }
}
