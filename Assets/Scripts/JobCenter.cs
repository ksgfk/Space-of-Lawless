using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace KSGFK
{
    public class JobCenter : IDisposable
    {
        private RegistryImpl<EntryJob> _jobInfo;
        private List<IJobWrapper> _jobList;

        public JobCenter()
        {
            GameManager.Instance.PerInit += OnGamePerInit;
            GameManager.Instance.Init += OnGameInit;
        }

        private static void OnGamePerInit() { }

        private void OnGameInit()
        {
            _jobInfo = new RegistryImpl<EntryJob>("job");
            foreach (var jobInfo in GameManager.MetaData.JobInfo)
            {
                foreach (var job in GameManager.TempData.Query<EntryJob>(GameManager.GetDataPath(jobInfo.Path)))
                {
                    _jobInfo.Register(job);
                }
            }

            _jobList = _jobInfo.Select(info =>
                {
                    var type = Type.GetType(info.FullTypeName);
                    if (type == null)
                    {
                        throw new ArgumentException();
                    }

                    if (!typeof(IJobWrapper).IsAssignableFrom(type))
                    {
                        throw new ArgumentException();
                    }

                    var instance = Activator.CreateInstance(type);
                    return instance as IJobWrapper;
                })
                .ToList();
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
            var index = _jobInfo[name];
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