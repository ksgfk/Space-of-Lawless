using System;
using System.Collections.Generic;
using MPipeline;
using Unity.Collections;

namespace KSGFK
{
    public abstract class JobTemplate<T> : IJobWrapper where T : unmanaged
    {
        private List<IJobCallback<JobTemplate<T>>> _callbacks;
        protected NativeList<T> DataList;

        public ref T this[int id] => ref DataList[id];

        public string Name { get; }

        protected JobTemplate(string name)
        {
            Name = name;
            _callbacks = new List<IJobCallback<JobTemplate<T>>>();
            DataList = new NativeList<T>(0, Allocator.Persistent);
        }

        public void Dispose()
        {
            DataList.Dispose();
            DataList = default;
            _callbacks = null;
        }

        public abstract void OnUpdate(float deltaTime);

        public int AddData(in T data, IJobCallback<JobTemplate<T>> callback)
        {
            var callList = _callbacks.Count;
            var index = callList == DataList.Length ? callList : throw new InvalidOperationException("可能有bug");
            DataList.Add(data);
            _callbacks.Add(callback);
            callback.DataId = index;
            callback.Job = this;
            return index;
        }

        public void RemoveData(int runtimeId)
        {
            if (runtimeId < 0)
            {
                return;
            }

            var callList = _callbacks.Count;
            var all = callList == DataList.Length ? callList : throw new InvalidOperationException("可能有bug");
            var last = all - 1;
            if (runtimeId != last)
            {
                _callbacks[runtimeId] = _callbacks[last];
                DataList[runtimeId] = DataList[last];
                _callbacks[runtimeId].DataId = runtimeId;
            }

            DataList.RemoveLast();
            _callbacks.RemoveAt(last);
        }

        public void RemoveData(IJobCallback<JobTemplate<T>> callback) { RemoveData(callback.DataId); }
    }
}