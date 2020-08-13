using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public class Registry : RegisterEntry
    {
        protected IList<RegisterEntry> Container;
        protected IDictionary<string, int> QueryDict;
        protected ISet<string> EntryNameSet;

        public sealed override string RegisterName { get; }

        public int Head { get; private set; }
        public bool IsLock { get; private set; }
        public int Count { get; protected set; }
        public Type EntryType { get; }

        public Registry(string name, ISet<string> entryNameSet, Type entryType)
        {
            RegisterName = name;
            Container = new List<RegisterEntry>();
            QueryDict = null;
            EntryNameSet = entryNameSet;
            IsLock = false;
            EntryType = entryType;
        }

        public virtual void Register(RegisterEntry entry)
        {
            if (entry.GetType() != EntryType)
            {
                throw new ArgumentException($"{entry.GetType()}不是:{EntryType}");
            }

            if (IsLock)
            {
                throw new InvalidOperationException($"注册表:{RegisterName}已锁定，不可再注册");
            }

            if (EntryNameSet.Contains(entry.RegisterName))
            {
                throw new InvalidOperationException($"重复的名字:{entry.RegisterName}");
            }

            Container.Add(entry);
            EntryNameSet.Add(entry.RegisterName);
        }

        public virtual void Register(object obj)
        {
            if (!(obj is RegisterEntry entry))
            {
                throw new ArgumentException($"{obj}不是:{typeof(RegisterEntry)}类型");
            }

            Register(entry);
        }

        public RegisterEntry QueryEntry(int runtimeId)
        {
            if (!IsLock)
            {
                throw new InvalidOperationException();
            }

            if (runtimeId >= Head && runtimeId < Head + Count)
            {
                return Container[runtimeId];
            }

            Debug.LogWarning("超出id范围");
            return null;
        }

        public RegisterEntry QueryEntry(string registerName)
        {
            RegisterEntry result;
            if (QueryDict.TryGetValue(registerName, out var id))
            {
                result = QueryEntry(id);
            }
            else
            {
                if (!IsLock)
                {
                    throw new InvalidOperationException();
                }

                Debug.LogWarningFormat("[注册表:{0}]不存在{1}", RegisterName, registerName);
                result = default;
            }

            return result;
        }

        public void Remap(List<RegisterEntry> newContainer, IDictionary<string, int> queryDict)
        {
            Head = newContainer.Count;
            Count = Container.Count;
            for (var i = 0; i < Count; i++)
            {
                Container[i].Remap(Head + i);
            }

            foreach (var entry in Container)
            {
                queryDict.Add(entry.RegisterName, entry.RuntimeId);
            }

            newContainer.AddRange(Container);
            EntryNameSet = null;
            Container = newContainer;
            QueryDict = queryDict;
            IsLock = true;
        }

        public sealed override bool Check(out string reason)
        {
            if (IsLock)
            {
                if (QueryDict.ContainsKey(RegisterName))
                {
                    reason = $"已经存在的注册表{RegisterName}";
                    return false;
                }

                reason = string.Empty;
                return true;
            }

            if (EntryNameSet.Contains(RegisterName))
            {
                reason = $"已经存在的注册表{RegisterName}";
                return false;
            }

            reason = string.Empty;
            return true;
        }
    }

    public class Registry<T> : Registry, IEnumerable<T> where T : RegisterEntry
    {
        public T this[int runtimeId] => (T) QueryEntry(runtimeId);
        public T this[string registerName] => (T) QueryEntry(registerName);

        public Registry(string name, ISet<string> entryNameSet) : base(name, entryNameSet, typeof(T)) { }

        public virtual void Register(T entry)
        {
            if (IsLock)
            {
                throw new InvalidOperationException($"注册表:{RegisterName}已锁定，不可再注册");
            }

            if (EntryNameSet.Contains(entry.RegisterName))
            {
                throw new InvalidOperationException($"重复的名字:{entry.RegisterName}");
            }

            if (entry.Check(out var reason))
            {
                Container.Add(entry);
                EntryNameSet.Add(entry.RegisterName);
            }
            else
            {
                Debug.LogWarning(reason);
            }
        }

        public override void Register(object obj)
        {
            if (!(obj is T entry))
            {
                throw new ArgumentException($"{obj}不是:{typeof(RegisterEntry)}类型");
            }

            Register(entry);
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (IsLock)
            {
                for (var i = 0; i < Count; i++)
                {
                    yield return (T) Container[Head + i];
                }
            }
            else
            {
                foreach (var entry in Container)
                {
                    yield return (T) entry;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}