using MPipeline;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace KSGFK
{
    public struct JobMoveInitReq
    {
        public float Speed;
        public Vector2 Direction;
    }

    public struct DataMove
    {
        public float Speed;
        public float2 Direction;
        public float2 Translation;
    }

    public class JobMove : JobWrapperImpl<JobMoveInitReq, DataMove>
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

        public override void OnUpdate()
        {
            new Move
            {
                DataList = DataList,
                DeltaTime = Time.deltaTime
            }.Run(DataList.Length);
        }

        protected override void OnAddData(ref JobMoveInitReq data)
        {
            DataList.Add(new DataMove {Direction = data.Direction, Speed = data.Speed});
        }
    }
}