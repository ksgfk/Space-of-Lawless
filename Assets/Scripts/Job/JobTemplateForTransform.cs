using System;
using System.Collections.Generic;
using MPipeline;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace KSGFK
{
    public abstract class JobTemplateForTransform<T> : IJobWrapper where T : unmanaged
    {
        protected TransformAccessArray TransArr;
        protected NativeList<T> DataArr;
        private List<IJobCallback<JobTemplateForTransform<T>>> _callbacks;
        private int _length;

        protected JobTemplateForTransform(string name, int size)
        {
            Name = name;
            TransArr = new TransformAccessArray(size, 8);
            DataArr = new NativeList<T>(size, Allocator.Persistent);
            _callbacks = new List<IJobCallback<JobTemplateForTransform<T>>>(size);
            _length = 0;
        }

        public string Name { get; }

        public abstract void OnUpdate(float deltaTime);

        public void Dispose()
        {
            TransArr.Dispose();
            DataArr.Dispose();
            _callbacks = null;
        }

        public void AddData(Transform trans, in T movData, IJobCallback<JobTemplateForTransform<T>> callback)
        {
            if (!trans) throw new ArgumentNullException(nameof(trans));
            callback.DataId = _length;
            callback.Job = this;
            TransArr.Add(trans);
            DataArr.Add(movData);
            _callbacks.Add(callback);
            _length++;
            CheckLength();
        }

        public void RemoveData(IJobCallback<JobTemplateForTransform<T>> callback)
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
            TransArr.RemoveAtSwapBack(removed);
            DataArr.Remove(removed);

            _length--;
            CheckLength();
        }

        private void CheckLength()
        {
            if (_length != TransArr.length ||
                _length != DataArr.Length ||
                _length != _callbacks.Count)
            {
                throw new ArgumentException();
            }
        }
    }
}