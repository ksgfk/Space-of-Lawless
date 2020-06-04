using MPipeline;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;

namespace KSGFK
{
    public class JobMove : JobTemplate<MoveData>
    {
        [BurstCompile]
        private struct Move : IJobParallelFor
        {
            public NativeList<MoveData> DataList;
            public float DeltaTime;

            public void Execute(int index)
            {
                ref var data = ref DataList[index];
                data.Translation = math.normalizesafe(data.Direction) * data.Speed * DeltaTime;
            }
        }

        public JobMove(string name) : base(name) { }

        protected override void PerUpdate(float deltaTime)
        {
            new Move
            {
                DataList = DataList,
                DeltaTime = deltaTime
            }.Run(DataList.Length);
        }
    }
}