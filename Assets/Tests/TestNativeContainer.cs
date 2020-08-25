using KSGFK;
using MPipeline;
using NUnit.Framework;
using Unity.Collections;

namespace Tests
{
    public class TestNativeContainer
    {
        [Test]
        public void TestPriorityQueue()
        {
            using (var pq = new NativePriorityQueue<int>(Allocator.Temp))
            {
                pq.Enqueue(2);
                pq.Enqueue(3);
                pq.Enqueue(1);
                pq.Enqueue(666);
                pq.Enqueue(23);
                pq.Enqueue(233);

                Assert.True(pq.Dequeue() == 1);
                Assert.True(pq.Dequeue() == 2);
                Assert.True(pq.Dequeue() == 3);
                Assert.True(pq.Dequeue() == 23);
                Assert.True(pq.Dequeue() == 233);
                Assert.True(pq.Dequeue() == 666);
            }
        }

        [Test]
        public void TestList()
        {
            using (var l = new NativeList<int>(5, Allocator.Temp))
            {
                l.Add(0);
                l.Add(1);
                l.Add(2);
                l.Add(3);
                l.Add(4);

                l.RemoveRange(2, 4);
                Assert.True(l.Length == 2);
                for (int i = 0; i < l.Length; i++)
                {
                    Assert.True(l[i] == i);
                }

                l.Add(2);
                l.Add(3);
                l.Add(4);
                l.Add(5);
                l.Add(6);
                l.Add(7);

                l.RemoveRange(2, 4);
                for (int i = 2; i < l.Length; i++)
                {
                    Assert.True(l[i] == i + 3);
                }
            }
        }

        [Test]
        public void TestQueue()
        {
            using (var q = new NativeQueue<int>(5, Allocator.Temp))
            {
                q.Enqueue(5);
                q.Enqueue(4);
                q.Enqueue(3);
                q.Enqueue(2);
                q.Enqueue(1);

                int count = 5;
                while (q.Length > 0)
                {
                    Assert.True(q.Length == count);
                    Assert.True(q.Dequeue() == count);
                    count--;
                }
            }
        }
    }
}