using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndexedDictionary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace IndexedDictionaryTests
{
    [TestClass()]
    public class DoubleKeyDictionaryTests
    {
        IEnumerable<Tuple<Guid, int>> GetRandomData(int count)
        {
            var random = new Random(100);
            while (count > 0)
            {
                yield return new Tuple<Guid, int>(Guid.NewGuid(), random.Next(1,1000));
                count -= 1;
            }
        }
            
        [TestMethod()]
        public void TestIndiciesPerformance()
        {
            var dictionary = new CompositeKeyDictionary<Guid, int, object>();
            var data = GetRandomData(1000000);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            foreach (var x in data)
            {
                dictionary.Add(x, x);
            }

            var timeToBuiltDictionaryWithIndicies = stopWatch.ElapsedMilliseconds;

            // Time to build Dict without indicies
            dictionary.Clear();
            dictionary.UseIndices = false;
            stopWatch.Restart();
            foreach (var x in data)
            {
                dictionary.Add(x, x);
            }
            var timeToBuiltDictionaryWithOutIndicies = stopWatch.ElapsedMilliseconds;

            //Time required to rebuilt two indicies
            stopWatch.Restart();
            dictionary.UseIndices = true;
            var timeToRebuiltIndices = stopWatch.ElapsedMilliseconds;

            // Get speed with index access
            stopWatch.Restart();
            var xs = dictionary[10].ToList();
            var timeToAccessWithIndices = stopWatch.ElapsedMilliseconds;

            // Now turn it off
            dictionary.UseIndices = false;
            stopWatch.Restart();
            xs = dictionary[10].ToList();
            var timeToAccessWithOutIndices = stopWatch.ElapsedMilliseconds;

            Trace.WriteLine("timeToBuiltDictionaryWithIndicies: " + timeToBuiltDictionaryWithIndicies);
            Trace.WriteLine("timeToBuiltDictionaryWithOutIndicies: " + timeToBuiltDictionaryWithOutIndicies);
            Trace.WriteLine("timeToRebuiltIndices: " + timeToRebuiltIndices);
            Trace.WriteLine("timeToAccessWithIndices: " + timeToAccessWithIndices);
            Trace.WriteLine("timeToAccessWithOutIndices: " + timeToAccessWithOutIndices);
        }
    }
}
