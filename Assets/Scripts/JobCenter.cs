using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public class JobCenter : IDisposable
    {
        private readonly List<IJobWrapper> _jobs;

        public JobCenter() { _jobs = new List<IJobWrapper>(); }

        public void OnUpdate()
        {
            var deltaTime = Time.deltaTime;
            foreach (var job in _jobs)
            {
                job.OnUpdate(deltaTime);
            }
        }

        public void AddJob(IJobWrapper wrapper)
        {
            if (_jobs.Find(job => job.Name == wrapper.Name) != default)
            {
                Debug.LogWarningFormat("已存在Job {0}", wrapper.Name);
                return;
            }

            _jobs.Add(wrapper);
        }

        public IJobWrapper GetJob(string name)
        {
            var findResult = _jobs.Find(job => job.Name == name);
            if (findResult == default)
            {
                Debug.LogWarningFormat("不存在Job {0}", name);
            }

            return findResult;
        }

        public T GetJob<T>(string name) where T : class, IJobWrapper { return GetJob(name) as T; }

        public void Dispose()
        {
            foreach (var job in _jobs)
            {
                job.Dispose();
            }

            _jobs.Clear();
        }
    }
}