using System;
using System.Collections.Generic;
using MPipeline;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace KSGFK
{
    public class JobMoveWithTransform : IJobWrapper
    {
        private TransformAccessArray _transArr;
        private NativeList<DataMoveWithTrans> _movData;
        private List<IJobCallback<JobMoveWithTransform>> _callbacks;
        private int _length;

        [BurstCompile]
        private struct MoveWithTransform : IJobParallelForTransform
        {
            public NativeList<DataMoveWithTrans> DataList;
            public float DeltaTime;

            public void Execute(int index, TransformAccess transform)
            {
                ref var data = ref DataList[index];
                var trans = math.normalizesafe(data.Direction) * data.Speed * DeltaTime;
                transform.position += new Vector3(trans.x, trans.y);
            }
        }

        public string Name { get; }

        public JobMoveWithTransform(string name, int size)
        {
            Name = name;
            _transArr = new TransformAccessArray(size, 8);
            _movData = new NativeList<DataMoveWithTrans>(size, Allocator.Persistent);
            _callbacks = new List<IJobCallback<JobMoveWithTransform>>(size);
            _length = 0;
        }

        public void OnUpdate(float deltaTime)
        {
            var handler = new MoveWithTransform
            {
                DataList = _movData,
                DeltaTime = deltaTime
            }.Schedule(_transArr);
            handler.Complete();
        }

        public void Dispose()
        {
            _transArr.Dispose();
            _movData.Dispose();
            _callbacks = null;
        }

        public void AddData(Transform trans, in DataMoveWithTrans movData, IJobCallback<JobMoveWithTransform> callback)
        {
            if (!trans) throw new ArgumentNullException(nameof(trans));
            callback.DataId = _length;
            callback.Job = this;
            _transArr.Add(trans);
            _movData.Add(movData);
            _callbacks.Add(callback);
            _length++;
            CheckLength();
        }

        public void RemoveData(IJobCallback<JobMoveWithTransform> callback)
        {
            var removed = callback.DataId;
            if (removed < 0 || removed >= _length)
            {
                throw new ArgumentException($"无效id:{removed}");
            }

            var lastIndex = _callbacks.GetLastIndex();
            var last = _callbacks.GetLastElement();
            if (last == default)
            {
                throw new ArgumentException();
            }

            last.DataId = removed;

            _callbacks[removed] = last;
            _callbacks.RemoveAt(lastIndex);
            _transArr.RemoveAtSwapBack(removed);
            _movData.Remove(removed);

            _length--;
            CheckLength();
        }

        private void CheckLength()
        {
            if (_length != _transArr.length ||
                _length != _movData.Length ||
                _length != _callbacks.Count)
            {
                throw new ArgumentException();
            }
        }
    }
}