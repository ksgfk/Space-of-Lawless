using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public class ObjectPool
    {
        private readonly GameObject _template;
        private readonly List<GameObject> _pool;
        private readonly Stack<int> _usable;

        public string Symbol { get; }

        public IReadOnlyList<GameObject> Container => _pool;

        public ObjectPool(GameObject template, int initCount, string symbol = null)
        {
            _template = template;
            Symbol = symbol;
            _pool = new List<GameObject>(initCount);
            _usable = new Stack<int>(initCount);
        }

        public GameObject Instantiate() { return UnityEngine.Object.Instantiate(_template); }

        public int Get()
        {
            int ptr;
            GameObject go;
            if (_usable.Count <= 0)
            {
                go = Instantiate();
                ptr = _pool.Count;
                _pool.Add(go);
            }
            else
            {
                ptr = _usable.Pop();
                go = _pool[ptr];
            }

            go.SetActive(true);
            return ptr;
        }

        public void Return(int ptr)
        {
            if (ptr >= _pool.Count) throw new ArgumentException();
            _usable.Push(ptr);
            var go = _pool[ptr];
            go.SetActive(false);
        }

        public int Get(Transform parent)
        {
            var ptr = Get();
            var go = _pool[ptr];
            go.transform.SetParent(parent);
            return ptr;
        }

        public int Get(in Vector3 pos, in Quaternion rot)
        {
            var ptr = Get();
            var go = _pool[ptr];
            go.transform.SetPositionAndRotation(pos, rot);
            return ptr;
        }

        public int Get(in Vector3 pos, in Quaternion rot, Transform parent)
        {
            var ptr = Get();
            var go = _pool[ptr];
            var trans = go.transform;
            trans.SetPositionAndRotation(pos, rot);
            trans.SetParent(parent);
            return ptr;
        }

        public int Get(Action<GameObject> action)
        {
            var ptr = Get();
            var go = _pool[ptr];
            action(go);
            return ptr;
        }

        public void ReAllocate(int count)
        {
            if (_pool.Count > count)
            {
                var m = Math.Max(0, count - 1);
                _pool.RemoveRange(m, _pool.Count - m);
            }
            else if (_pool.Count < count)
            {
                for (var i = 0; i < count - _pool.Count; i++)
                {
                    _pool.Add(Instantiate());
                }
            }
        }
    }
}