using MPipeline;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;

namespace KSGFK
{
    public class JobMove : JobTemplate<DataMove>
    {
        [BurstCompile]
        private struct Move : IJobParallelFor
        {
            public NativeList<DataMove> DataList;
            public float DeltaTime;

            public void Execute(int index)
            {
                ref var data = ref DataList[index];
                data.Translation = math.normalizesafe(data.Direction) * data.Speed * DeltaTime;
            }
        }

        public JobMove(string name) : base(name) { }

        public override void OnUpdate(float deltaTime)
        {
            new Move
            {
                DataList = DataList,
                DeltaTime = deltaTime
            }.Run(DataList.Length);
        }
    }
}