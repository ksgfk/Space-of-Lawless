using System;
using System.Collections.Generic;
using MPipeline;
using Unity.Collections;
using UnityEngine.Profiling;

namespace KSGFK
{
    public abstract class JobTemplate<T> : IJobWrapper where T : unmanaged
    {
        private List<IJobCallback<T>> _callbacks;
        protected NativeList<T> DataList;

        public ref T this[int id] => ref DataList[id];

        public string Name { get; }

        protected JobTemplate(string name)
        {
            Name = name;
            _callbacks = new List<IJobCallback<T>>();
            DataList = new NativeList<T>(0, Allocator.Persistent);
        }

        public void Dispose()
        {
            DataList.Dispose();
            DataList = default;
            _callbacks = null;
        }

        protected abstract void PerUpdate(float deltaTime);

        public virtual void OnUpdate(float deltaTime)
        {
            PerUpdate(deltaTime);
            Profiler.BeginSample(Name);
            var actionBuffer = new ActionBuffer();
            foreach (var callback in _callbacks)
            {
                callback.JobUpdate(ref this[callback.DataId], ref actionBuffer);
            }

            actionBuffer.Action();
            Profiler.EndSample();
        }

        public int AddData(in T data, IJobCallback<T> callback)
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

        public void RemoveData(IJobCallback<T> callback) { RemoveData(callback.DataId); }
    }
}