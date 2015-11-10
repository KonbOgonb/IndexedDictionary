using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexedDictionaryTests
{
    public class Person
    {
        public class MyId : IEquatable<MyId>
        {
            public int Grade { get; set; }
            public Guid Guid { get; set; }

            public bool Equals(MyId other)
            {
                return Guid.Equals(other.Guid) && Grade.Equals(other.Grade);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as MyId);
            }

            public override int GetHashCode()
            {
                return 17 * Grade.GetHashCode() + 23 * Guid.GetHashCode();
            }
        }
        public MyId Id { get; set; }
        public string Name { get; set; }
    }
}
