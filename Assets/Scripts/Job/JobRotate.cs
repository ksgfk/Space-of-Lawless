using System;
using MPipeline;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace KSGFK
{
    public struct JobRotateInput
    {
        public Transform Trans;
        public Vector2 Standard;
        public Vector2 Direction;
    }

    public struct JobRotateOutput
    {
        public float2 Standard;
        public float2 Direction;
    }

    public class JobRotate : JobWrapper<JobRotateInput, JobRotateOutput>
    {
        private TransformAccessArray _transArr;
        private static readonly Transform[] Empty = Array.Empty<Transform>();

        [BurstCompile]
        private struct Rotate : IJobParallelForTransform
        {
            public NativeList<JobRotateOutput> Data;

            public void Execute(int index, TransformAccess transform)
            {
                ref var data = ref Data[index];
                var degree = MathExt.SignedAngle(data.Standard.xy0(), data.Direction.xy0(), new float3(0, 0, 1));
                transform.rotation = quaternion.AxisAngle(new float3(0, 0, 1), degree);
            }
        }

        public JobRotate() { _transArr = new TransformAccessArray(0, 4); }

        protected override JobRotateOutput ConvertData(ref JobRotateInput input)
        {
            return new JobRotateOutput {Standard = input.Standard, Direction = input.Direction};
        }

        public override JobInfo<JobRotateOutput> AddValue(JobRotateInput value)
        {
            var data = ConvertData(ref value);
            DataList.Add(data);
            _transArr.Add(value.Trans);
            return null;
        }

        public override void AddValue(JobRotateInput value, JobInfo<JobRotateOutput> returnValue)
        {
            var data = ConvertData(ref value);
            DataList.Add(data);
            _transArr.Add(value.Trans);
        }

        public override void RemoveValue(JobInfo<JobRotateOutput> info) { throw new InvalidOperationException(); }

        protected override JobHandle Update()
        {
            var handle = new Rotate
            {
                Data = DataList
            }.Schedule(_transArr);
            return handle;
        }

        public override void AfterUpdate()
        {
            DataList.Clear();
            _transArr.SetTransforms(Empty);
        }

        public override void Dispose()
        {
            base.Dispose();
            _transArr.Dispose();
        }
    }
}