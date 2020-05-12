using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public class JobCenter : IDisposable
    {
        private readonly List<IJobWrapper> _jobs;

        public IJobWrapper this[int id] => _jobs[id];

        public JobCenter() { _jobs = new List<IJobWrapper>(); }

        public int AddJob(IJobWrapper wrapper)
        {
            var id = _jobs.Count;
            wrapper.RuntimeId = id;
            _jobs.Add(wrapper);
            return id;
        }

        public void OnUpdate()
        {
            var deltaTime = Time.deltaTime;
            foreach (var job in _jobs)
            {
                job.OnUpdate(deltaTime);
            }
        }

        public void Dispose(int id)
        {
            _jobs[id].Dispose();
            _jobs[id] = null;
            _jobs.Swap(id, _jobs.GetLastIndex());
            _jobs.RemoveAt(_jobs.GetLastIndex());
        }

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