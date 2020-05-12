using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public class JobCenter : IDisposable
    {
        private readonly Dictionary<string, IJobWrapper> _jobs;

        public JobCenter() { _jobs = new Dictionary<string, IJobWrapper>(); }

        public void OnUpdate()
        {
            var deltaTime = Time.deltaTime;
            foreach (var job in _jobs.Values)
            {
                job.OnUpdate(deltaTime);
            }
        }

        public void AddJob(IJobWrapper wrapper) { _jobs.Add(wrapper.Name, wrapper); }

        public IJobWrapper GetJob(string name)
        {
            if (!_jobs.TryGetValue(name, out var result))
            {
                Debug.LogWarningFormat("不存在Job {0}", name);
            }

            return result;
        }

        public T GetJob<T>(string name) where T : class, IJobWrapper { return GetJob(name) as T; }

        public void Dispose()
        {
            foreach (var job in _jobs.Values)
            {
                job.Dispose();
            }

            _jobs.Clear();
        }
    }
}