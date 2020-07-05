using System;
using CsvHelper.Configuration.Attributes;

namespace KSGFK
{
    /// <summary>
    /// 继承自Mono的注册项基类
    /// </summary>
    [Serializable]
    public abstract class EntryIdentity<T> : IStageProcessEntry where T : IdentityObject
    {
        private int _runtimeId = int.MinValue;
        [ReflectionInject] protected string name = null;

        public int RuntimeId
        {
            get => _runtimeId;
            set => _runtimeId = Helper.SingleAssign(value, _runtimeId != int.MinValue);
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