using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public class PoolCenter
    {
        private readonly List<ObjectPool> _pools;
        private readonly Dictionary<string, (int, Action<int>)> _indexer;

        public PoolCenter()
        {
            _pools = new List<ObjectPool>();
            _indexer = new Dictionary<string, (int, Action<int>)>();
        }

        public int Allocate(GameObject template, string name, int initCount = 0, Action<int> onIndexChange = null)
        {
            int index;
            ObjectPool pool;
            if (_indexer.TryGetValue(name, out var data))
            {
                pool = _pools[data.Item1];
                index = data.Item1;
                if (pool.Container.Count < initCount)
                {
                    pool.ReAllocate(initCount);
                }

                if (onIndexChange != null)
                {
                    data.Item2 += onIndexChange;
                }
            }
            else
            {
                index = _pools.Count;
                pool = new ObjectPool(template, initCount, name);
                _pools.Add(pool);
                _indexer.Add(name, (index, onIndexChange));
            }

            return index;
        }

        public bool IsAllocated(string name) { return _indexer.ContainsKey(name); }

        public void Release(int index)
        {
            var lastIndex = _pools.GetLastIndex();
            var last = _pools[lastIndex];
            var (_, onIndexChange) = _indexer[last.Symbol];
            onIndexChange(index);
            var removed = _pools[index];
            _indexer.Remove(removed.Symbol);
            _pools[index] = _pools[lastIndex];
            _pools.RemoveAt(lastIndex);
        }

        public void Release(string name)
        {
            var (index, _) = _indexer[name];
            Release(index);
        }

        public int Get(int id) { return _pools[id].Get(); }

        public int Get(int id, Transform parent) { return _pools[id].Get(parent); }

        public int Get(int id, in Vector3 pos, in Quaternion rot) { return _pools[id].Get(in pos, in rot); }

        public int Get(int id, in Vector3 pos, in Quaternion rot, Transform parent)
        {
            return _pools[id].Get(in pos, in rot, parent);
        }

        public int Get(int id, Action<GameObject> action) { return _pools[id].Get(action); }

        public void Return(int poolId, int objId) { _pools[poolId].Return(objId); }
    }
}