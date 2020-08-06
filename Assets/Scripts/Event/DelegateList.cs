using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public class DelegateList : ICollection<Delegate>
    {
        private readonly List<Delegate> _delegates;
        private readonly Type _delegateType;

        public int Count => _delegates.Count;
        public bool IsReadOnly => false;

        public DelegateList(Type delegateType)
        {
            _delegates = new List<Delegate>();
            _delegateType = delegateType;
        }

        public IEnumerator<Delegate> GetEnumerator()
        {
            foreach (var dg in _delegates)
            {
                yield return dg;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public void Add(Delegate item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.GetType() != _delegateType) throw new ArgumentException($"委托{item.GetType()}类型不是{_delegateType}");
            _delegates.Add(item);
        }

        public void Clear() { _delegates.Clear(); }

        public bool Contains(Delegate item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.GetType() != _delegateType) throw new ArgumentException($"委托{item.GetType()}类型不是{_delegateType}");
            foreach (var dg in _delegates)
            {
                if (dg == item)
                {
                    return true;
                }
            }

            return false;
        }

        public void CopyTo(Delegate[] array, int arrayIndex) { _delegates.CopyTo(array, arrayIndex); }

        public bool Remove(Delegate item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.GetType() != _delegateType) throw new ArgumentException($"委托{item.GetType()}类型不是{_delegateType}");
            for (var i = 0; i < Count; i++)
            {
                if (_delegates[i] == item)
                {
                    _delegates.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void Invoke(params object[] obj)
        {
            foreach (var dg in _delegates)
            {
                try
                {
                    dg.DynamicInvoke(obj);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
    }
}