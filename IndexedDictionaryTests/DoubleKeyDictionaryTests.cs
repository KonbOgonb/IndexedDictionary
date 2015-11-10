using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndexedDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexedDictionary.Tests
{
    [TestClass()]
    public class DoubleKeyDictionaryTests
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

        Person firstPerson = new Person { Id = new Person.MyId { Grade = 1, Guid = new Guid("c5b6797a-217b-4628-9859-1821da5d67c4") }, Name = "Vasya" };
        Person secondPerson = new Person { Id = new Person.MyId { Grade = 2, Guid = new Guid("c5b6797a-217b-4628-9859-1821da5d67c4") }, Name = "Vasya" };
        Person thirdPerson = new Person { Id = new Person.MyId { Grade = 2, Guid = new Guid("c5b6797a-217b-4628-9859-1821da5d67c4") }, Name = "Petya" };
        Person fourthPerson = new Person { Id = new Person.MyId { Grade = 2, Guid = new Guid("3141d23a-a2f3-44b1-b61b-2bcba201ec11") }, Name = "Petya" };

        [TestMethod()]
        public void TestFirstIndex()
        {
            List<Person> personsData = new List<Person>()
            {
                firstPerson, secondPerson, thirdPerson,fourthPerson
            };
            var dictionary = new DoubleKeyDictionary<Person.MyId, string, Person>();
            personsData.ForEach(p => dictionary.Add(new Tuple<Person.MyId, string>(p.Id, p.Name), p));
            var xs = dictionary[secondPerson.Id];

            Assert.IsTrue(xs.First().Name != xs.Last().Name);
            Assert.AreEqual(xs.Count(), 2);

            dictionary.Remove(new Tuple<Person.MyId, string>(secondPerson.Id, secondPerson.Name));
            xs = dictionary[secondPerson.Id];
            Assert.IsTrue(xs.First().Name == "Petya");
            Assert.AreEqual(xs.Count(), 1);
        }

        [TestMethod()]
        public void TestSecondIndex()
        {
            List<Person> personsData = new List<Person>()
            {
                firstPerson, secondPerson, thirdPerson,fourthPerson
            };
            var dictionary = new DoubleKeyDictionary<Person.MyId, string, Person>();
            personsData.ForEach(p => dictionary.Add(new Tuple<Person.MyId, string>(p.Id, p.Name), p));
            var xs = dictionary["Vasya"];

            Assert.IsTrue(xs.First().Name == xs.Last().Name);
            Assert.AreEqual(xs.Count(), 2);

            dictionary.Remove(new Tuple<Person.MyId, string>(secondPerson.Id, secondPerson.Name));
            xs = dictionary["Vasya"];
            Assert.IsTrue(xs.First().Id.Equals(firstPerson.Id));
            Assert.AreEqual(xs.Count(), 1);
        }
    }
}