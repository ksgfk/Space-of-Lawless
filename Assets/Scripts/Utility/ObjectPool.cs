using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    /// <summary>
    /// 简易对象池
    /// </summary>
    public class ObjectPool
    {
        private readonly GameObject _template;
        private readonly List<GameObject> _pool;
        private readonly Stack<int> _usable;
        private readonly List<bool> _isUsed;

        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string UniqueId { get; }

        /// <summary>
        /// 内部容器
        /// </summary>
        public IReadOnlyList<GameObject> Container => _pool;

        /// <summary>
        /// 对象模板
        /// </summary>
        public GameObject Template => _template;

        public ObjectPool(GameObject template, int initCount = 0, string uniqueId = "")
        {
            _template = template;
            UniqueId = uniqueId;
            _pool = new List<GameObject>(initCount);
            _usable = new Stack<int>(initCount);
            _isUsed = new List<bool>(initCount);
            for (var i = 0; i < initCount; i++)
            {
                Get();
            }

            for (var i = 0; i < initCount; i++)
            {
                Return(i);
            }
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
                _isUsed.Add(false);
            }
            else
            {
                ptr = _usable.Pop();
                go = _pool[ptr];
            }

            _isUsed[ptr] = true;
            go.SetActive(true);
            return ptr;
        }

        public void Return(int ptr)
        {
            if (!_isUsed[ptr])
            {
                throw new InvalidOperationException($"[池:{UniqueId}]:对象{ptr}已回收");
            }

            _usable.Push(ptr);
            var go = _pool[ptr];
            go.SetActive(false);
            _isUsed[ptr] = false;
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

        /// <summary>
        /// 释放池中所有对象
        /// </summary>
        public void Release(bool uncheck = false, Action<GameObject> releaseAction = null)
        {
            if (!uncheck)
            {
                if (_pool.Count != _usable.Count)
                {
                    throw new InvalidOperationException(
                        $"[池:{UniqueId}]:对象未全部回收,当前池内:{_usable.Count},共分配{_pool.Count}");
                }
            }

            if (releaseAction == null)
            {
                foreach (var go in _pool)
                {
                    UnityEngine.Object.Destroy(go);
                }
            }
            else
            {
                foreach (var go in _pool)
                {
                    releaseAction(go);
                }
            }

            _pool.Clear();
            _usable.Clear();
            _isUsed.Clear();
        }
    }
}