using System;
using System.Collections;
using System.Collections.Generic;

namespace KSGFK
{
    public class DisorderPool<T> : IEnumerable<T>
    {
        private readonly List<T> _pool;
        private readonly SortedSet<int> _sort;

        public DisorderPool()
        {
            _pool = new List<T>();
            _sort = new SortedSet<int>();
        }

        public int PutIn(T value)
        {
            int pointer;
            if (_sort.Count <= 0)
            {
                pointer = _pool.Count;
                _pool.Add(value);
            }
            else
            {
                pointer = _sort.Min;
                _pool[pointer] = value;
                _sort.Remove(pointer);
            }

            return pointer;
        }

        public T TakeOut(int pointer)
        {
            if (pointer < 0 || pointer >= _pool.Count) throw new InvalidOperationException();
            if (!_sort.Add(pointer))
            {
                throw new ArgumentException();
            }

            var result = _pool[pointer];
            _pool[pointer] = default;
            return result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var t in _pool)
            {
                yield return t;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public void Clear()
        {
            for (var i = 0; i < _pool.Count; i++)
            {
                _pool[i] = default;
                _sort.Add(i);
            }
        }
    }
}