using System;
using System.Collections.Generic;
using MPipeline;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace KSGFK
{
    public sealed class NormalJobTimingTask : JobWrapper
    {
        private readonly struct TaskInfo : IComparable<TaskInfo>
        {
            public readonly float ExecuteTime;
            public readonly int TaskIndex;
            public readonly int TaskGen;

            public TaskInfo(float executeTime, int taskIndex, int taskGen)
            {
                ExecuteTime = executeTime;
                TaskIndex = taskIndex;
                TaskGen = taskGen;
            }

            public int CompareTo(TaskInfo other) { return ExecuteTime.CompareTo(other.ExecuteTime); }
        }

        private struct InternalTask
        {
            public Action Task;
            public int Gen;

            public InternalTask(Action task, int gen)
            {
                Task = task;
                Gen = gen;
            }
        }

        private NativePriorityQueue<TaskInfo> _timeQueue;
        private NativeList<TaskInfo> _executeIndex;
        private readonly List<InternalTask> _taskList;
        private int _usableIndex;
        private bool _canModify;

        [BurstCompile]
        private struct InternalTimingTask : IJob
        {
            public NativePriorityQueue<TaskInfo> TaskQueue;
            public NativeList<TaskInfo> ExeIndex;
            public float NowTime;

            public void Execute()
            {
                while (TaskQueue.Count > 0)
                {
                    ref var top = ref TaskQueue.Peek();
                    if (NowTime >= top.ExecuteTime)
                    {
                        ExeIndex.Add(ref top);
                        TaskQueue.Dequeue();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public NormalJobTimingTask()
        {
            _timeQueue = new NativePriorityQueue<TaskInfo>(Allocator.Persistent);
            _executeIndex = new NativeList<TaskInfo>(0, Allocator.Persistent);
            _taskList = new List<InternalTask>();
            _usableIndex = 0;
            _canModify = true;
        }

        public override JobHandle OnUpdate()
        {
            if (_timeQueue.Count <= 0)
            {
                return default;
            }

            return new InternalTimingTask
            {
                TaskQueue = _timeQueue,
                ExeIndex = _executeIndex,
                NowTime = Time.time
            }.Schedule();
        }

        public override void AfterUpdate()
        {
            _canModify = false;
            for (var i = 0; i < _executeIndex.Length; i++)
            {
                ref var taskInfo = ref _executeIndex[i];
                var task = _taskList[taskInfo.TaskIndex];
                if (task.Gen == taskInfo.TaskGen)
                {
                    try
                    {
                        task.Task?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }

            for (var i = 0; i < _executeIndex.Length; i++)
            {
                ref var index = ref _executeIndex[i];
                _taskList[index.TaskIndex] = new InternalTask(null, index.TaskGen);
                if (index.TaskIndex < _usableIndex)
                {
                    _usableIndex = index.TaskIndex;
                }
            }

            _executeIndex.Clear();
            _canModify = true;
        }

        public override void Dispose()
        {
            _timeQueue.Dispose();
            _executeIndex.Dispose();
            _taskList.Clear();
        }

        public JobInfo AddTask(float delay, Action task)
        {
            var info = new JobInfo(null, -1);
            AddTask(delay, task, info);
            return info;
        }

        public void AddTask(float delay, Action task, JobInfo info)
        {
            if (task == null) throw new ArgumentNullException();
            if (!_canModify) throw new InvalidOperationException();
            var taskIndex = _usableIndex;
            if (taskIndex >= _taskList.Count)
            {
                _taskList.Add(new InternalTask(null, 0));
            }

            var t = _taskList[taskIndex];
            t.Gen = t.Gen == int.MaxValue ? 0 : t.Gen + 1;
            t.Task = task;
            _taskList[taskIndex] = t;

            FindNextUsableIndex();

            _timeQueue.Enqueue(new TaskInfo(Time.time + delay, taskIndex, t.Gen));
            info.SetWrapper(this);
            info.SetIndex(taskIndex);
        }

        private void FindNextUsableIndex()
        {
            _usableIndex++;
            while (true)
            {
                if (_usableIndex >= _taskList.Count)
                {
                    break;
                }

                var t = _taskList[_usableIndex];
                if (t.Task != null)
                {
                    _usableIndex++;
                }
                else
                {
                    break;
                }
            }
        }

        public void RemoveTask(JobInfo info)
        {
            if (info.Wrapper != this) throw new InvalidOperationException();
            if (!_canModify) throw new InvalidOperationException();
            var t = _taskList[info.Index];
            t.Task = null;
            _taskList[info.Index] = t;
            if (info.Index < _usableIndex)
            {
                _usableIndex = info.Index;
            }
        }
    }
}