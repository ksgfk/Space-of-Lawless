using MPipeline;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace KSGFK
{
    public struct JobTranslateInput
    {
        public Transform Transform;
        public Vector2 Velocity;
    }

    public struct JobTranslateOutput
    {
        public float2 Velocity;
        public float2 DeltaMove;
    }

    public class JobTranslate : JobWrapper<JobTranslateInput, JobTranslateOutput>
    {
        private TransformAccessArray _transArr;

        [BurstCompile]
        private struct Translate : IJobParallelForTransform
        {
            public NativeList<JobTranslateOutput> Data;
            public float DeltaTime;

            public void Execute(int index, TransformAccess transform)
            {
                ref var data = ref Data[index];
                var d = data.Velocity * DeltaTime;
                float3 lastPos = transform.position;
                quaternion rot = transform.rotation;
                var nextPos = lastPos + MathExt.TransformDirection(rot, new float3(d, 0));
                transform.position = nextPos;
                data.DeltaMove = (nextPos - lastPos).xy;
            }
        }

        public JobTranslate() { _transArr = new TransformAccessArray(0, 4); }

        protected override JobTranslateOutput ConvertData(ref JobTranslateInput input)
        {
            return new JobTranslateOutput {Velocity = input.Velocity};
        }

        public override void AddValue(JobTranslateInput value, JobInfo<JobTranslateOutput> returnValue)
        {
            base.AddValue(value, returnValue);
            _transArr.Add(value.Transform);
        }

        public override void RemoveValue(JobInfo<JobTranslateOutput> info)
        {
            base.RemoveValue(info);
            _transArr.RemoveAtSwapBack(info.Index);
        }

        public override void OnUpdate()
        {
            var handle = new Translate
            {
                Data = DataList,
                DeltaTime = Time.deltaTime
            }.Schedule(_transArr);
            handle.Complete();
        }

        public override void Dispose()
        {
            base.Dispose();
            _transArr.Dispose();
        }
    }
}