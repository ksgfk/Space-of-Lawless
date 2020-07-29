using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public interface IRegistry<T> : IEnumerable<T> where T : IRegisterEntry
    {
        string RegistryName { get; }

        T this[int id] { get; }

        T this[string name] { get; }

        void Register(T registerEntry);
    }

    public class RegistryImpl<T> : IRegistry<T> where T : IRegisterEntry
    {
        private readonly List<T> _entries;
        private readonly Dictionary<string, int> _index;

        public string RegistryName { get; }

        public T this[int id] => _entries[id];

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

        public void Register(T registerEntry)
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

        public IEnumerator<T> GetEnumerator() { return ((IEnumerable<T>) _entries).GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}