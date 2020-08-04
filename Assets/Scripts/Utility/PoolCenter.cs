using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KSGFK
{
    public class PoolCenter : IDisposable
    {
        private readonly List<ObjectPool> _pools;
        private readonly Dictionary<string, int> _indexer;

        public ObjectPool this[int index] => _pools[index];
        public ObjectPool this[string name] => _pools[_indexer[name]];

        public PoolCenter()
        {
            _pools = new List<ObjectPool>();
            _indexer = new Dictionary<string, int>();
        }

        public int Allocate(GameObject template, string name, int initCount = 0)
        {
            if (!_indexer.TryGetValue(name, out var index))
            {
                index = _pools.Count;
                var pool = new ObjectPool(template, initCount, name);
                _pools.Add(pool);
                _indexer.Add(name, index);
            }

            return index;
        }

        public bool IsAllocated(string name) { return _indexer.ContainsKey(name); }

        public int Get(int id) { return _pools[id].Get(); }

        public int Get(int id, Transform parent) { return _pools[id].Get(parent); }

        public int Get(int id, in Vector3 pos, in Quaternion rot) { return _pools[id].Get(in pos, in rot); }

        public int Get(int id, in Vector3 pos, in Quaternion rot, Transform parent)
        {
            return _pools[id].Get(in pos, in rot, parent);
        }

        public int Get(int id, Action<GameObject> action) { return _pools[id].Get(action); }

        public void Return(int poolId, int objId) { _pools[poolId].Return(objId); }

        public void Dispose()
        {
            foreach (var pool in _pools)
            {
                pool.Release(true);
            }
        }
    }
}