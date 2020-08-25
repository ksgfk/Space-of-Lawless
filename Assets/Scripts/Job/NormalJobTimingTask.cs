using System;
using MPipeline;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace KSGFK
{
    public sealed unsafe class NormalJobTimingTask : JobWrapper
    {
        private struct TaskInfo : IComparable<TaskInfo>, IFunction<TaskInfo, int>
        {
            public float Time;
            public int Pointer;

            public int CompareTo(TaskInfo other) { return Time.CompareTo(other.Time); }
            public int Run(ref TaskInfo a) { return Time.CompareTo(a.Time); }
        }

        [BurstCompile]
        private struct TimingTask : IJob
        {
            public NativeList<TaskInfo> WaitingList;
            public NativeQueue<TaskInfo> AddCache;
            public NativeQueue<int> RmCache;
            [NativeDisableUnsafePtrRestriction] public NativeList_Int* ExeList;
            public float NowTime;

            public void Execute()
            {
                if (AddCache.Length > 0)
                {
                    AddTask();
                }

                if (RmCache.Length > 0)
                {
                    RemoveTask();
                }

                if (WaitingList.Length > 0)
                {
                    SelectTask();
                }
            }

            private void AddTask()
            {
                while (AddCache.TryDequeue(out var taskInfo))
                {
                    WaitingList.Add(ref taskInfo);
                }

                MathExt.Quicksort(WaitingList.unsafePtr, 0, WaitingList.Length - 1);
            }

            private void RemoveTask()
            {
                while (RmCache.TryDequeue(out var ptr))
                {
                    for (int i = 0; i < WaitingList.Length; i++)
                    {
                        ref var info = ref WaitingList[i];
                        if (info.Pointer == ptr)
                        {
                            WaitingList.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            private void SelectTask()
            {
                var end = -1;
                for (var i = 0; i < WaitingList.Length; i++)
                {
                    ref var task = ref WaitingList[i];
                    if (task.Time <= NowTime)
                    {
                        end = i;
                    }
                    else
                    {
                        break;
                    }
                }

                if (end <= -1)
                {
                    return;
                }

                *ExeList = new NativeList_Int(end + 1, Allocator.TempJob);
                for (var i = 0; i <= end; i++)
                {
                    ExeList->Add(WaitingList[i].Pointer);
                }
            }
        }

        /// <summary>
        /// 等待倒计时的任务列表
        /// </summary>
        private NativeList<TaskInfo> _waitingList;

        /// <summary>
        /// 添加任务缓存
        /// </summary>
        private NativeQueue<TaskInfo> _addCache;

        /// <summary>
        /// 删除任务缓存
        /// </summary>
        private NativeQueue<int> _rmCache;

        /// <summary>
        /// 可以执行的任务
        /// </summary>
        private NativeList_Int _exeList;

        /// <summary>
        /// 任务储存池
        /// </summary>
        private readonly DisorderPool<Action> _tasks;

        public NormalJobTimingTask()
        {
            _waitingList = new NativeList<TaskInfo>(0, Allocator.Persistent);
            _addCache = new NativeQueue<TaskInfo>(0, Allocator.Persistent);
            _rmCache = new NativeQueue<int>(0, Allocator.Persistent);
            _tasks = new DisorderPool<Action>();
            _exeList = new NativeList_Int();
        }

        public override JobHandle OnUpdate()
        {
            fixed (NativeList_Int* p = &_exeList)
            {
                if (_addCache.Length > 0 || _rmCache.Length > 0 || _waitingList.Length > 0)
                {
                    return new TimingTask
                    {
                        WaitingList = _waitingList,
                        AddCache = _addCache,
                        ExeList = p,
                        NowTime = Time.time,
                        RmCache = _rmCache
                    }.Schedule();
                }
                else
                {
                    return default;
                }
            }
        }

        public override void AfterUpdate()
        {
            if (_exeList.isCreated)
            {
                for (var i = 0; i < _exeList.Length; i++)
                {
                    _tasks.TakeOut(_exeList[i]).Invoke();
                }

                _waitingList.RemoveRange(0, _exeList.Length - 1);
                _exeList.Dispose();
            }
        }

        public override void Dispose()
        {
            _waitingList.Dispose();
            _addCache.Dispose();
            _rmCache.Dispose();
            _tasks.Clear();
            _exeList.Dispose();
        }

        public JobInfo AddTask(float delay, Action task)
        {
            var info = JobInfo.Default;
            AddTask(delay, task, info);
            return info;
        }

        public void AddTask(float delay, Action task, JobInfo info)
        {
            var pointer = _tasks.PutIn(task);
            _addCache.Enqueue(new TaskInfo {Time = Time.time + delay, Pointer = pointer});
            info.SetIndex(pointer);
            info.SetWrapper(this);
        }

        public void RemoveTask(JobInfo info)
        {
            if (info.Wrapper != this) throw new ArgumentException();
            _rmCache.Enqueue(info.Index);
        }
    }
}