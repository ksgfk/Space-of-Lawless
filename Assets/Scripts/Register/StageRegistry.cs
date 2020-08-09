using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KSGFK
{
    public class StageRegistry<T> : Registry<T>, IStageRegistry where T : RegisterEntry, IStageProcess
    {
        private SortedSet<T> _wait;

        public StageRegistry(string name, ISet<string> entryNameSet) : base(name, entryNameSet)
        {
            _wait = new SortedSet<T>();
        }

        public void AddToWaitRegister(T entry)
        {
            if (_wait == null) throw new InvalidOperationException("Init阶段才能注册实体");
            if (!_wait.Add(entry))
            {
                throw new ArgumentException($"重复的id:{entry.RegisterName}");
            }
        }

        public void AddToWaitRegister(object obj)
        {
            if (obj is T entry)
            {
                AddToWaitRegister(entry);
            }
            else
            {
                throw new ArgumentException($"错误的类型:{obj.GetType()}");
            }
        }

        public override void Register(T entry) { AddToWaitRegister(entry); }

        public override void Register(object obj) { AddToWaitRegister(obj); }

        public override void Register(RegisterEntry entry) { AddToWaitRegister(entry); }

        public async Task PreProcessEntry()
        {
            foreach (var task in _wait.Select(entry => entry.PreProcess()).Where(task => task != null))
            {
                await task;
            }
        }

        public void RegisterAll()
        {
            foreach (var entry in _wait)
            {
                try
                {
                    entry.Process();
                    if (!entry.Check(out var info))
                    {
                        throw new ArgumentException(info);
                    }

                    base.Register(entry);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            _wait = null;
        }
    }
}