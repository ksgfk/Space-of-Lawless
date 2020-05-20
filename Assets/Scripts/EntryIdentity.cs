using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public abstract class EntryIdentity<T> : IStageProcessEntry where T : IdentityObject
    {
        [SerializeField] private int runtimeId = int.MinValue;
        [SerializeField] private string name = null;

        public int RuntimeId
        {
            get => runtimeId;
            set => runtimeId = Helper.SingleAssign(value, runtimeId != int.MinValue);
        }

        public string RegisterName => name;

        public abstract void PerProcess();

        public abstract void Process();

        public abstract bool Check(out string info);

        protected abstract T InstantiateBehavior();

        public T Instantiate()
        {
            var result = InstantiateBehavior();
            result.RuntimeId = RuntimeId;
            return result;
        }

        public virtual void Destroy(T instance) { UnityEngine.Object.Destroy(instance.gameObject); }
    }
}