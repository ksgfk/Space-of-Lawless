using System;
using System.Collections;
using System.Collections.Generic;
using MPipeline;
using Unity.Collections;

namespace KSGFK
{
    public struct NativePriorityQueue<T> : IReadOnlyCollection<T>, IDisposable where T : unmanaged, IComparable<T>
    {
        private NativeList<T> _heap;

        public int Count => _heap.Length - 1;

        public NativePriorityQueue(Allocator allocator) { _heap = new NativeList<T>(1, allocator) {default}; }

        public void Dispose() { _heap.Dispose(); }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 1; i < _heap.Length; i++)
            {
                yield return _heap[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public void Enqueue(T item)
        {
            _heap.Add(item);
            var now = Count;
            while (now > 1)
            {
                var next = now >> 1;
                if (_heap[now].CompareTo(_heap[next]) >= 0)
                {
                    return;
                }

                var temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;
                now = next;
            }
        }

        public ref T Dequeue()
        {
            if (Count <= 0)
            {
                _heap[0] = default;
                return ref _heap[0];
            }

            _heap[0] = _heap[1]; //取出堆顶
            _heap[1] = _heap[Count]; //将末尾放入到堆顶
            _heap.Remove(Count); //删除末尾
            var now = 1;
            while (now * 2 <= Count)
            {
                var next = now * 2;
                if (next < Count && _heap[next + 1].CompareTo(_heap[next]) < 0)
                {
                    next++;
                }

                if (_heap[now].CompareTo(_heap[next]) <= 0)
                {
                    return ref _heap[0];
                }

                var temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;
                now = next;
            }

            return ref _heap[0];
        }

        public ref T Peek() { return ref _heap[1]; }
    }
}