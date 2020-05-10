using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public interface IRegistry<T>
    {
        string RegistryName { get; }

        IEntry<T> this[int id] { get; }

        IEntry<T> this[string name] { get; }

        void Register(IEntry<T> entry);
    }

    public class RegistryImpl<T> : IRegistry<T>
    {
        private readonly List<IEntry<T>> _entries;
        private readonly Dictionary<string, int> _index;

        public string RegistryName { get; }

        public IEntry<T> this[int id] => _entries[id];

        public IEntry<T> this[string name]
        {
            get
            {
                IEntry<T> result;
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

        public RegistryImpl(string registryName)
        {
            RegistryName = registryName;
            _entries = new List<IEntry<T>>();
            _index = new Dictionary<string, int>();
        }

        public void Register(IEntry<T> entry)
        {
            if (_index.ContainsKey(entry.RegisterName))
            {
                Debug.LogWarningFormat("[注册表:{0}]已注册过的元素{1}", RegistryName, entry.RegisterName);
                return;
            }

            var id = _entries.Count;
            entry.Id = id;
            _entries.Add(entry);
            _index.Add(entry.RegisterName, id);
        }
    }
}