using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndexedDictionary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading;

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
                yield return new Tuple<Guid, int>(Guid.NewGuid(), random.Next(1, busketSize));
                count -= 1;
            }
        }

        const int dictSize = 1000000;
        const int busketSize = 10000;

        [TestMethod()]
        public void TestIndiciesPerformance()
        {
            var dictionary = new CompositeKeyDictionary<Guid, int, object>();
            var data = GetRandomData(dictSize);

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
            var xs = dictionary[data.First().Item2].ToList();
            var timeToAccessWithIndices = stopWatch.ElapsedMilliseconds;

            // Now turn it off
            dictionary.UseIndices = false;
            stopWatch.Restart();
            xs = dictionary[data.First().Item2].ToList();
            var timeToAccessWithOutIndices = stopWatch.ElapsedMilliseconds;

            Trace.WriteLine(string.Format("Performance results for dictSize = {0} and busketSize = {1}", dictSize, busketSize));
            Trace.WriteLine("timeToBuiltDictionaryWithIndicies: " + timeToBuiltDictionaryWithIndicies);
            Trace.WriteLine("timeToBuiltDictionaryWithOutIndicies: " + timeToBuiltDictionaryWithOutIndicies);
            Trace.WriteLine("timeToRebuiltIndices: " + timeToRebuiltIndices);
            Trace.WriteLine("timeToAccessWithIndices: " + timeToAccessWithIndices);
            Trace.WriteLine("timeToAccessWithOutIndices: " + timeToAccessWithOutIndices);
            Trace.WriteLine("----------------------------------------------------------------");
        }

        const int tasksCount = 100;
        const int stepsPerTask = 100;

        [TestMethod()]
        public void ConcurrencyTest()
        {
            //test adding elements
            var dictionary = new CompositeKeyDictionary<Guid, int, object>();
            var tasks = new Task[tasksCount];
            var ids = new Tuple<Guid,int>[tasksCount * stepsPerTask];
            for (int x = 0; x < tasksCount; ++x)
            {
                int xCopy = x;
                tasks[xCopy] = (new Task(() =>
                {
                    int step = 0;
                    foreach (var data in GetRandomData(stepsPerTask))
                    {
                        dictionary[data] = data;
                        //we need this collection for further removal
                        ids[xCopy * stepsPerTask + step] = data;
                        step += 1;
                    }
                }));
            }

            foreach(var t in tasks)
            {
                t.Start();
            }

            Task.WaitAll(tasks);
            Assert.AreEqual(stepsPerTask * tasksCount, dictionary.Count());

            //test removing elements
            for (int x = 0; x < tasksCount; x++)
            {
                int xCopy = x;
                tasks[xCopy] = (new Task(() =>
                {
                    int removalsRemaining = stepsPerTask;
                    while (removalsRemaining > 0)
                    {
                        bool result = dictionary.Remove(ids[stepsPerTask * xCopy + removalsRemaining - 1]);
                        if (result)
                        {
                            removalsRemaining -= 1;
                        }
                    }
                }));
            }

            foreach (var t in tasks)
            {
                t.Start();
            }

            Task.WaitAll(tasks);
            Assert.AreEqual(0, dictionary.Count());
        }
    }
}
