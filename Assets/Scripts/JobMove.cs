using System;
using System.Collections.Generic;
using MPipeline;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Profiling;

namespace KSGFK
{
    public unsafe class JobMove : IJobWrapper
    {
        private List<IDataCallback> _callbacks;
        private NativeList<MoveData> _data;

        public ref MoveData this[int id] => ref _data[id];

        [BurstCompile]
        private struct Move : IJobParallelFor
        {
            public NativeList<MoveData> DataList;
            public float DeltaTime;

            public void Execute(int index)
            {
                ref var data = ref DataList[index];
                data.Translation = math.normalizesafe(data.Direction) * data.Speed * DeltaTime;
            }
        }

        public string Name { get; }

        public JobMove(string name)
        {
            Name = name;
            _callbacks = new List<IDataCallback>();
            _data = new NativeList<MoveData>(0, Allocator.Persistent);
        }

        public void Dispose()
        {
            _data.Dispose();
            _data = default;
            _callbacks = null;
        }

        private static readonly string ProfilerName = $"{typeof(JobMove).FullName}.Update";

        public void OnUpdate(float deltaTime)
        {
            new Move
            {
                DataList = _data,
                DeltaTime = deltaTime
            }.Run(_data.Length);
            Profiler.BeginSample(ProfilerName);
            foreach (var callback in _callbacks)
            {
                callback.OnUpdate();
            }

            Profiler.EndSample();
        }

        public int AddData(in MoveData data, IDataCallback callback)
        {
            var callList = _callbacks.Count;
            var index = callList == _data.Length ? callList : throw new InvalidOperationException("可能有bug");
            _data.Add(data);
            _callbacks.Add(callback);
            callback.JobWrapper = this;
            return index;
        }

        public void RemoveData(int runtimeId)
        {
            var callList = _callbacks.Count;
            var all = callList == _data.Length ? callList : throw new InvalidOperationException("可能有bug");
            var last = all - 1;
            if (runtimeId != last)
            {
                _callbacks[runtimeId] = _callbacks[last];
                _data[runtimeId] = _data[last];
                _callbacks[runtimeId].DataId = runtimeId;
            }

            _data.RemoveLast();
            _callbacks.RemoveAt(last);
        }
    }
}