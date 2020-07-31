using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    /// <summary>
    /// 阶段注册
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StageRegistry<T> : RegistryImpl<T> where T : IStageProcessEntry
    {
        private List<T> _waitList;

        public StageRegistry(string registryName) : base(registryName) { _waitList = new List<T>(); }

        /// <summary>
        /// 预处理，加入注册队列
        /// </summary>
        public void AddToWaitRegister(T registerEntry)
        {
            if (_waitList == null) throw new InvalidOperationException("Init阶段才能注册实体");
            registerEntry.PerProcess();
            _waitList.Add(registerEntry);
        }

        public void AddToWaitRegister(object obj)
        {
            if (obj is T entry)
            {
                AddToWaitRegister(entry);
            }
            else
            {
                throw new InvalidOperationException($"类型不匹配,需要{typeof(T).FullName},传入{obj.GetType().FullName}");
            }
        }

        public override void Register(T registerEntry) { AddToWaitRegister(registerEntry); }

        public override void Register(object obj) { AddToWaitRegister(obj); }

        /// <summary>
        /// 检查所有注册队列的项，并注册符合条件的项
        /// </summary>
        public void RegisterAll()
        {
            foreach (var entry in _waitList)
            {
                try
                {
                    entry.Process();
                    if (!entry.Check(out var info))
                    {
                        Debug.LogWarningFormat("{0}未通过注册检查,原因 {1}", entry.RegisterName, info);
                        continue;
                    }

                    base.Register(entry);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            _waitList = null;
        }
    }
}