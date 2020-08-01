using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public interface IRegistry<T> : IEnumerable<T> where T : IRegisterEntry
    {
        string RegistryName { get; }

        /// <summary>
        /// 使用数字id查询，返回值可能是null
        /// </summary>
        T this[int id] { get; }

        /// <summary>
        /// 使用字符串id查询，返回值可能是null
        /// </summary>
        /// <param name="name"></param>
        T this[string name] { get; }

        void Register(T registerEntry);

        void Register(object obj);
    }

    public class RegistryImpl<T> : IRegistry<T> where T : IRegisterEntry
    {
        private readonly List<T> _entries;
        private readonly Dictionary<string, int> _index;

        public string RegistryName { get; }

        public T this[int id]
        {
            get
            {
                if (!_entries.TryIndex(id, out var entry))
                {
                    Debug.LogWarningFormat("[注册表:{0}]不存在Id:{1}", RegistryName, id);
                }

                return entry;
            }
        }

        public T this[string name]
        {
            get
            {
                T result;
                if (_index.TryGetValue(name, out var id))
                {
                    result = _entries[id];
                }
                else
                {
                    Debug.LogWarningFormat("[注册表:{0}]不存在{1}", RegistryName, name);
                    result = default;
                }

                return result;
            }
        }

        public IReadOnlyDictionary<string, int> NameToIndex => _index;

        public RegistryImpl(string registryName)
        {
            RegistryName = registryName;
            _entries = new List<T>();
            _index = new Dictionary<string, int>();
        }

        public virtual void Register(T registerEntry)
        {
            if (_index.ContainsKey(registerEntry.RegisterName))
            {
                Debug.LogWarningFormat("[注册表:{0}]已注册过的元素{1}", RegistryName, registerEntry.RegisterName);
                return;
            }

            var id = _entries.Count;
            registerEntry.RuntimeId = id;
            _entries.Add(registerEntry);
            _index.Add(registerEntry.RegisterName, id);
        }

        public virtual void Register(object obj)
        {
            if (obj is T entry)
            {
                Register(entry);
            }
            else
            {
                throw new InvalidOperationException($"类型不匹配,需要{typeof(T).FullName},传入{obj.GetType().FullName}");
            }
        }

        public IEnumerator<T> GetEnumerator() { return ((IEnumerable<T>) _entries).GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}