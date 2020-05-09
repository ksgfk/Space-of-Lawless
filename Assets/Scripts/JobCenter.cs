using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KSGFK
{
    public class JobCenter : IDisposable
    {
        private readonly List<IJobWrapper> _jobs;

        public IJobWrapper this[int id]
        {
            get
            {
                var job = _jobs[id];
                return job.CanAccess ? job : throw new InvalidOperationException();
            }
        }

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
                if (job.CanAccess)
                {
                    job.OnUpdate(deltaTime);
                }
            }
        }

        public void Dispose(int id) { this[id].Dispose(); }

        public void Dispose()
        {
            foreach (var job in _jobs)
            {
                job.Dispose();
            }
        }
    }
}