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

        public virtual void Process() { }

        public abstract bool Check(out string info);

        /// <summary>
        /// 实例化注册项的行为
        /// </summary>
        protected abstract T InstantiateBehavior();

        /// <summary>
        /// 实例化注册项
        /// </summary>
        public T Instantiate()
        {
            var result = InstantiateBehavior();
            result.RuntimeId = RuntimeId;
            return result;
        }

        /// <summary>
        /// 销毁注册项实例
        /// </summary>
        /// <param name="instance"></param>
        public virtual void Destroy(T instance) { UnityEngine.Object.Destroy(instance.gameObject); }
    }
}