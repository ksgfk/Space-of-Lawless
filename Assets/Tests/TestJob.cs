using KSGFK;
using NUnit.Framework;
using Unity.Collections;

namespace Tests
{
    public class TestJob
    {
        [Test]
        public void TestJobPasses()
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
    }
}