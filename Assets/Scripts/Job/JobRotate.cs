using MPipeline;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace KSGFK
{
    public struct JobRotateInitReq
    {
        // public float Speed;
        public Vector2 Delta;
    }

    public struct DataRotate
    {
        // public float Speed;
        public float2 Delta;
        public quaternion Rotation;
    }

    public class JobRotate : JobWrapperImpl<JobRotateInitReq, DataRotate>
    {
        [BurstCompile]
        private struct Rotate : IJobParallelFor
        {
            public NativeList<DataRotate> DataList;
            // public float DeltaTime;

            public void Execute(int index)
            {
                ref var data = ref DataList[index];
                ref var delta = ref data.Delta;
                if (math.abs(delta.x) < 0.0000001f && math.abs(delta.y) < 0.0000001f)
                {
                    return;
                }

                var vec = new float3(delta, 0);
                // var result = MathExt.FromToRotation(new float3(0, 1, 0), vec);
                // data.Rotation = math.slerp(data.Rotation, result, data.Speed * DeltaTime);
                data.Rotation = MathExt.FromToRotation(new float3(0, 1, 0), vec);
            }
        }

        public override void OnUpdate()
        {
            new Rotate
            {
                DataList = DataList,
                // DeltaTime = Time.deltaTime
            }.Run(DataList.Length);
        }

        protected override void OnAddData(ref JobRotateInitReq data)
        {
            DataList.Add(new DataRotate {Delta = data.Delta});
        }
    }
}