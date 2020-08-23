using System;
using System.Collections.Generic;
using MPipeline;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace KSGFK
{
    /// <summary>
    /// TODO:提供删除任务API
    /// </summary>
    public sealed class NormalJobTimingTask : JobWrapper
    {
        private readonly struct TaskInfo : IComparable<TaskInfo>
        {
            public readonly float ExecuteTime;
            public readonly int TaskIndex;

            public TaskInfo(float executeTime, int taskIndex)
            {
                ExecuteTime = executeTime;
                TaskIndex = taskIndex;
            }

            public int CompareTo(TaskInfo other) { return ExecuteTime.CompareTo(other.ExecuteTime); }
        }

        private NativePriorityQueue<TaskInfo> _timeQueue;
        private NativeList<int> _executeIndex;
        private readonly List<Action> _taskList;
        private int _usableIndex;

        [BurstCompile]
        private struct InternalTimingTask : IJob
        {
            public NativePriorityQueue<TaskInfo> TaskQueue;
            public NativeList<int> ExeIndex;
            public float NowTime;

            public void Execute()
            {
                while (TaskQueue.Count > 0)
                {
                    ref var top = ref TaskQueue.Peek();
                    if (NowTime >= top.ExecuteTime)
                    {
                        ExeIndex.Add(top.TaskIndex);
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
            _executeIndex = new NativeList<int>(0, Allocator.Persistent);
            _taskList = new List<Action>();
            _usableIndex = 0;
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
            for (var i = 0; i < _executeIndex.Length; i++)
            {
                try
                {
                    _taskList[_executeIndex[i]]();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            for (var i = 0; i < _executeIndex.Length; i++)
            {
                var index = _executeIndex[i];
                _taskList[index] = null;
                if (index < _usableIndex)
                {
                    _usableIndex = index;
                }
            }

            _executeIndex.Clear();
        }

        public override void Dispose()
        {
            _timeQueue.Dispose();
            _executeIndex.Dispose();
            _taskList.Clear();
        }

        public void AddTask(float delay, Action task)
        {
            if (task == null) throw new ArgumentNullException();
            var taskIndex = _usableIndex;
            if (taskIndex >= _taskList.Count)
            {
                _taskList.Add(null);
            }

            _taskList[taskIndex] = task;
            FindNextUsableIndex();
            _timeQueue.Enqueue(new TaskInfo(Time.time + delay, taskIndex));
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

                if (_taskList[_usableIndex] != null)
                {
                    _usableIndex++;
                }
                else
                {
                    break;
                }
            }
        }
    }
}