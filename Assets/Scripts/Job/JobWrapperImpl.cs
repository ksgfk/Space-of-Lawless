using System;
using System.Collections.Generic;
using MPipeline;
using Unity.Collections;

namespace KSGFK
{
    public abstract class JobWrapperImpl<TInput, TStore> : IJobWrapper
        where TInput : struct
        where TStore : unmanaged
    {
        protected NativeList<TStore> DataList;
        protected readonly List<JobInfo> JobInfo;

        public int DataCount => DataList.Length;
        public int JobCount => JobInfo.Count;
        public ref TStore this[JobInfo info] => ref DataList[info.Id];

        protected JobWrapperImpl()
        {
            DataList = new NativeList<TStore>(0, Allocator.Persistent);
            JobInfo = new List<JobInfo>();
        }

        public abstract void OnUpdate();

        public JobInfo AddData(TInput data)
        {
            var dataIndex = JobInfo.Count;
            OnAddData(ref data);
            var info = new JobInfo(this, dataIndex);
            JobInfo.Add(info);
            if (!CheckLength())
            {
                throw new ArgumentException();
            }

            return info;
        }

        protected abstract void OnAddData(ref TInput data);

        public void RemoveData(JobInfo info)
        {
            if (info.Job != this)
            {
                return;
            }

            var id = info.Id;
            if (id < 0 || id >= JobInfo.Count)
            {
                throw new ArgumentException($"无效id:{id}");
            }

            DataList.Remove(id);
            OnRemoveData(id);
            var lastIndex = JobInfo.Count - 1;
            if (lastIndex == id)
            {
                JobInfo.RemoveAt(id);
            }
            else if (lastIndex <= -1)
            {
                JobInfo.RemoveAt(0);
            }
            else
            {
                var lastElement = JobInfo[lastIndex];
                lastElement.Id = id;
                JobInfo[id] = lastElement;
                JobInfo.RemoveAt(lastIndex);
            }

            OnRemoveJobInfo(info);
            if (!CheckLength())
            {
                throw new ArgumentException();
            }
        }

        protected virtual void OnRemoveData(int id) { }

        public virtual void Dispose()
        {
            DataList.Dispose();
            foreach (var info in JobInfo)
            {
                OnRemoveJobInfo(info);
            }

            JobInfo.Clear();
        }

        private static void OnRemoveJobInfo(JobInfo info)
        {
            info.Id = -1;
            info.Job = null;
        }

        protected virtual bool CheckLength() { return JobInfo.Count == DataList.Length; }
    }
}