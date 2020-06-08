using System;
using System.Collections.Generic;
using MPipeline;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace KSGFK
{
    public class JobTimingTask : IJobWrapper
    {
        private List<IJobTimingTask> _tasks;
        private NativeList<DataTriggerTime> _data;
        private NativeList<int> _triggerList;
        private int _length;

        [BurstCompile]
        private struct CheckTime : IJob
        {
            public NativeList<DataTriggerTime> DataList;
            public float NowTime;
            public NativeList<int> TriggerList;

            public void Execute()
            {
                for (var i = 0; i < DataList.Length; i++)
                {
                    ref var data = ref DataList[i];
                    if (NowTime >= data.Time)
                    {
                        TriggerList.Add(i);
                    }
                }
            }
        }

        public string Name { get; }

        public JobTimingTask(string name)
        {
            Name = name;
            _tasks = new List<IJobTimingTask>();
            _data = new NativeList<DataTriggerTime>(0, Allocator.Persistent);
            _triggerList = new NativeList<int>(0, Allocator.Persistent);
            _length = 0;
        }

        public void OnUpdate(float deltaTime)
        {
            new CheckTime
            {
                DataList = _data,
                NowTime = Time.time,
                TriggerList = _triggerList
            }.Run();
            for (var i = 0; i < _triggerList.Length; i++)
            {
                _tasks[_triggerList[i]].RunTask();
            }

            for (var i = _triggerList.Length - 1; i >= 0; i--)
            {
                RemoveTask(_triggerList[i]);
            }

            _triggerList.Clear();
        }

        public void Dispose()
        {
            _data.Dispose();
            _triggerList.Dispose();
            _tasks = null;
        }

        public void AddTask(float expireTime, IJobTimingTask task)
        {
            task.DataId = _length;
            task.Job = this;
            _tasks.Add(task);
            _data.Add(new DataTriggerTime {Time = Time.time + expireTime});
            _length++;
            CheckLength();
        }

        private void RemoveTask(int removed)
        {
            if (removed < 0 || removed >= _length)
            {
                throw new ArgumentException($"无效id:{removed}");
            }

            var lastIndex = _tasks.GetLastIndex();
            var last = _tasks.GetLastElement();
            if (last == default)
            {
                throw new ArgumentException();
            }

            last.DataId = removed;
            _tasks[removed] = last;
            _tasks.RemoveAt(lastIndex);
            _data.Remove(removed);
            _length--;
            CheckLength();
        }

        public void RemoveTask(IJobTimingTask task) { RemoveTask(task.DataId); }

        private void CheckLength()
        {
            if (_length != _tasks.Count || _length != _data.Length)
            {
                throw new ArgumentException();
            }
        }
    }
}