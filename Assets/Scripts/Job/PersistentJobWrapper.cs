using System;
using System.Collections.Generic;
using MPipeline;
using Unity.Collections;
using Unity.Jobs;

namespace KSGFK
{
    /// <summary>
    /// 持久化工作
    /// </summary>
    public abstract class PersistentJobWrapper<TInput, TOutput> : JobWrapper
        where TInput : struct
        where TOutput : unmanaged
    {
        private NativeList<TOutput> _dataList;
        private readonly List<JobInfo> _jobInfos;

        protected NativeList<TOutput> DataList => _dataList;
        protected List<JobInfo> JobInfoList => _jobInfos;

        protected PersistentJobWrapper()
        {
            _dataList = new NativeList<TOutput>(0, Allocator.Persistent);
            _jobInfos = new List<JobInfo>();
        }

        public virtual JobInfo AddValue(TInput value)
        {
            var newJobInfo = new JobInfo(null, -1);
            AddValue(value, newJobInfo);
            return _jobInfos[newJobInfo.Index];
        }

        public virtual void AddValue(TInput value, JobInfo returnValue)
        {
            var data = ConvertData(ref value);
            var index = _dataList.Length;
            _dataList.Add(data);
            returnValue.SetWrapper(this);
            returnValue.SetIndex(index);
            _jobInfos.Add(returnValue);
        }

        protected abstract TOutput ConvertData(ref TInput input);

        public void RemoveValue(JobInfo info)
        {
            if (info.Wrapper != this) throw new ArgumentException();
            var index = info.Index;
            _dataList.Remove(index);

            OnRemoveValue(info);

            var lastIndex = _jobInfos.Count - 1;
            _jobInfos[index].Release();
            _jobInfos[index] = _jobInfos[lastIndex];
            _jobInfos.RemoveAt(lastIndex);
            if (index != lastIndex)
            {
                _jobInfos[index].SetIndex(index);
            }
        }

        protected virtual void OnRemoveValue(JobInfo info) { }

        public ref TOutput GetValue(JobInfo info)
        {
            if (info.Wrapper != this) throw new ArgumentException();
            return ref _dataList[info.Index];
        }

        public sealed override JobHandle OnUpdate() { return DataList.Length <= 0 ? default : UpdateBehaviour(); }

        protected abstract JobHandle UpdateBehaviour();

        public override void Dispose()
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