using System;
using MPipeline;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using static Unity.Mathematics.math;
using static Unity.Mathematics.quaternion;

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

    public sealed class TempJobRotate : TempJobWrapper<JobRotateInput, JobRotateOutput>
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
                transform.rotation = EulerZXY(0, 0, radians(degree));
            }
        }

        public TempJobRotate() { _transArr = new TransformAccessArray(0, 1); }

        public override void AddValue(JobRotateInput input)
        {
            base.AddValue(input);
            _transArr.Add(input.Trans);
        }

        protected override JobRotateOutput ConvertData(ref JobRotateInput input)
        {
            return new JobRotateOutput {Standard = input.Standard, Direction = input.Direction};
        }

        protected override JobHandle Update()
        {
            return new Rotate
            {
                Data = DataList
            }.Schedule(_transArr);
        }

        public override void AfterUpdate()
        {
            base.AfterUpdate();
            _transArr.SetTransforms(Empty);
        }

        public override void Dispose()
        {
            base.Dispose();
            _transArr.Dispose();
        }
    }
}