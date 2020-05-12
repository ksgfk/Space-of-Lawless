using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public interface IRegistry<T>
    {
        string RegistryName { get; }

        IRegisterEntry this[int id] { get; }

        IRegisterEntry this[string name] { get; }

        void Register(IRegisterEntry registerEntry);
    }

    public class RegistryImpl<T> : IRegistry<T>
    {
        private readonly List<IRegisterEntry> _entries;
        private readonly Dictionary<string, int> _index;

        public string RegistryName { get; }

        public IRegisterEntry this[int id] => _entries[id];

        public IRegisterEntry this[string name]
        {
            get
            {
                IRegisterEntry result;
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
            _entries = new List<IRegisterEntry>();
            _index = new Dictionary<string, int>();
        }

        public void Register(IRegisterEntry registerEntry)
        {
            if (_index.ContainsKey(registerEntry.RegisterName))
            {
                Debug.LogWarningFormat("[注册表:{0}]已注册过的元素{1}", RegistryName, registerEntry.RegisterName);
                return;
            }

            var id = _entries.Count;
            registerEntry.Id = id;
            _entries.Add(registerEntry);
            _index.Add(registerEntry.RegisterName, id);
        }
    }
}