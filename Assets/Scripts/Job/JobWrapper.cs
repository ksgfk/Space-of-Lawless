using System;
using System.Collections.Generic;
using MPipeline;
using Unity.Collections;
using Unity.Jobs;

namespace KSGFK
{
    public abstract unsafe class JobWrapper<TInput, TOutput> : IDisposable
        where TInput : struct
        where TOutput : unmanaged
    {
        private NativeList<TOutput> _dataList;
        private readonly List<JobInfo<TOutput>> _jobInfos;

        protected NativeList<TOutput> DataList => _dataList;
        protected List<JobInfo<TOutput>> JobInfoList => _jobInfos;

        protected JobWrapper()
        {
            _dataList = new NativeList<TOutput>(0, Allocator.Persistent);
            _jobInfos = new List<JobInfo<TOutput>>();
        }

        public virtual JobInfo<TOutput> AddValue(TInput value)
        {
            var newJobInfo = new JobInfo<TOutput>(default, -1);
            AddValue(value, newJobInfo);
            return _jobInfos[newJobInfo.Index];
        }

        public virtual void AddValue(TInput value, JobInfo<TOutput> returnValue)
        {
            var data = ConvertData(ref value);
            var index = _dataList.Length;
            _dataList.Add(data);
            returnValue.SetPointer(_dataList);
            returnValue.SetIndex(index);
            _jobInfos.Add(returnValue);
        }

        protected abstract TOutput ConvertData(ref TInput input);

        public virtual void RemoveValue(JobInfo<TOutput> info)
        {
            if (info.Pointer != _dataList.unsafePtr) throw new ArgumentException();
            var index = info.Index;
            _dataList.Remove(index);

            var lastIndex = _jobInfos.Count - 1;
            _jobInfos[index].Release();
            _jobInfos[index] = _jobInfos[lastIndex];
            _jobInfos.RemoveAt(lastIndex);
            _jobInfos[index].SetIndex(index);
        }

        public ref TOutput GetValue(JobInfo<TOutput> info)
        {
            if (info.Pointer != _dataList.unsafePtr) throw new ArgumentException();
            return ref _dataList[info.Index];
        }

        public JobHandle OnUpdate()
        {
            if (DataList.Length <= 0)
            {
                return default;
            }

            return Update();
        }

        protected abstract JobHandle Update();

        public virtual void AfterUpdate() { }

        public virtual void Dispose()
        {
            DataList.Dispose();
            foreach (var jobInfo in JobInfoList)
            {
                jobInfo.Release();
            }

            JobInfoList.Clear();
        }
    }
}