using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public class StageRegistry<T> : RegistryImpl<T>
    {
        private readonly IProcessor _processor;
        private List<IEntry<T>> _waitList;

        public StageRegistry(string registryName, IProcessor processor) : base(registryName)
        {
            _waitList = new List<IEntry<T>>();
            _processor = processor;
        }

        public void AddToWaitRegister(IEntry<T> entry)
        {
            if (_waitList == null) throw new InvalidOperationException("Init阶段才能注册实体");
            _processor.ProProcess(entry);
            _waitList.Add(entry);
        }

        public void RegisterAll()
        {
            var successCount = 0;
            foreach (var entry in _waitList)
            {
                try
                {
                    _processor.Process(entry);
                    if (!_processor.Check(entry, out var info))
                    {
                        Debug.LogWarningFormat("{0}未通过注册检查,原因 {1}", entry.RegisterName, info);
                        continue;
                    }

                    Register(entry);
                    successCount += 1;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            Debug.LogFormat("[注册表:{0}][数量:{1}]", RegistryName, successCount);
            _waitList = null;
        }
    }
}