using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace KSGFK
{
    public class JobCenter : IDisposable
    {
        private readonly GameManager _gm;
        private List<IJobWrapper> _jobList;

        public JobCenter(GameManager gm)
        {
            _gm = gm;
            gm.SetCallbackBeforePreInit += OnRegisterComplete;
        }

        private void OnRegisterComplete()
        {
            _gm.Register.RegisterComplete += () =>
            {
                _jobList = GameManager.Instance
                    .Register
                    .Job
                    .Select(entryJob => (IJobWrapper) Activator.CreateInstance(entryJob.JobType))
                    .ToList();
            };
        }

        public void OnUpdate()
        {
            foreach (var job in _jobList)
            {
                Profiler.BeginSample(job.GetType().FullName);
                job.OnUpdate();
                Profiler.EndSample();
            }
        }

        public IJobWrapper GetJob(string name)
        {
            var index = _gm.Register.Job[name];
            return _jobList[index.RuntimeId];
        }

        public IJobWrapper GetJob(int id) { return _jobList[id]; }

        public JobWrapperImpl<TInput, TStore> GetJob<TInput, TStore>(string name)
            where TInput : struct
            where TStore : unmanaged
        {
            var job = GetJob(name);
            JobWrapperImpl<TInput, TStore> result;
            if (job is JobWrapperImpl<TInput, TStore> impl)
            {
                result = impl;
            }
            else
            {
                Debug.LogWarningFormat("Job {0}的类型是 {1} ,不是 {2}",
                    name,
                    job.GetType(),
                    typeof(JobWrapperImpl<TInput, TStore>));
                result = default;
            }

            return result;
        }

        public void Dispose()
        {
            if (_jobList == null)
            {
                return;
            }

            foreach (var job in _jobList)
            {
                job.Dispose();
            }

            _jobList.Clear();
        }
    }
}