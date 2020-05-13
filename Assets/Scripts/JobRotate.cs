using MPipeline;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;

namespace KSGFK
{
    public class JobRotate : JobTemplate<RotateData>
    {
        [BurstCompile]
        private struct Rotate : IJobParallelFor
        {
            public NativeList<RotateData> DataList;
            public float DeltaTime;

            public void Execute(int index)
            {
                ref var data = ref DataList[index];
                var vec = data.Target - data.NowPos;
                vec.z = 0;
                var result = MathExt.FromToRotation(new float3(0, 1, 0), vec);
                data.Rotation = math.slerp(data.Rotation, result, data.Speed * DeltaTime);
            }
        }

        public JobRotate(string name) : base(name) { }

        protected override void PerUpdate(float deltaTime)
        {
            new Rotate
            {
                DataList = DataList,
                DeltaTime = deltaTime
            }.Run(DataList.Length);
        }
    }
}