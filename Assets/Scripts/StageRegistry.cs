using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public class StageRegistry<T> : RegistryImpl<T>
    {
        private List<IStageProcessEntry> _waitList;

        public StageRegistry(string registryName) : base(registryName)
        {
            _waitList = new List<IStageProcessEntry>();
        }

        public void AddToWaitRegister(IStageProcessEntry registerEntry)
        {
            if (_waitList == null) throw new InvalidOperationException("Init阶段才能注册实体");
            registerEntry.PerProcess();
            _waitList.Add(registerEntry);
        }

        public void RegisterAll()
        {
            var successCount = 0;
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