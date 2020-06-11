using System;
using System.Collections.Generic;
using MPipeline;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace KSGFK
{
    public struct JobTimingTaskInitReq
    {
        public float Duration;
        public Action Task;
    }

    public class JobTimingTask : JobWrapperImpl<JobTimingTaskInitReq, float>
    {
        private readonly List<Action> _tasks;
        private NativeList<int> _triggerList;

        public int TaskCount => _tasks.Count;

        [BurstCompile]
        private struct CheckTime : IJob
        {
            public NativeList<float> DataList;
            public float NowTime;
            public NativeList<int> TriggerList;

            public void Execute()
            {
                for (var i = 0; i < DataList.Length; i++)
                {
                    var data = DataList[i];
                    if (NowTime >= data)
                    {
                        TriggerList.Add(i);
                    }
                }
            }
        }

        public JobTimingTask()
        {
            _tasks = new List<Action>();
            _triggerList = new NativeList<int>(0, Allocator.Persistent);
        }

        public override void OnUpdate()
        {
            new CheckTime
            {
                DataList = DataList,
                NowTime = Time.time,
                TriggerList = _triggerList
            }.Run();
            for (var i = 0; i < _triggerList.Length; i++)
            {
                _tasks[_triggerList[i]]();
            }

            for (var i = _triggerList.Length - 1; i >= 0; i--)
            {
                RemoveData(JobInfo[_triggerList[i]]);
            }

            _triggerList.Clear();
        }

        protected override void OnAddData(ref JobTimingTaskInitReq data)
        {
            DataList.Add(data.Duration + Time.time);
            _tasks.Add(data.Task ?? (() => { }));
        }

        protected override void OnRemoveData(int id) { _tasks.RemoveAtSwapBack(id); }

        protected override bool CheckLength() { return base.CheckLength() && JobInfo.Count == _tasks.Count; }

        public override void Dispose()
        {
            base.Dispose();
            _triggerList.Dispose();
            _tasks.Clear();
        }
    }
}