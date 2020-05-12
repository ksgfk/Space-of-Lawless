using System.Collections.Generic;
using MPipeline;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace KSGFK
{
    public class JobMove : IJobWrapper
    {
        private readonly Dictionary<int, int> _redirect;
        private NativeList<MoveData> _data;

        public ref MoveData this[int id] => ref _data[id];

        [BurstCompile]
        private struct Move : IJobParallelFor
        {
            public NativeList<MoveData> DataList;
            public float DeltaTime;

            public void Execute(int index)
            {
                ref var data = ref DataList[index];
                data.Translation = math.normalize(data.Direction) * data.Speed * DeltaTime;
            }
        }

        public int RuntimeId { get; set; }

        public JobMove()
        {
            _redirect = new Dictionary<int, int>();
            _data = new NativeList<MoveData>(0, Allocator.Persistent);
        }

        public void Dispose()
        {
            _data.Dispose();
            _data = default;
        }

        public void OnUpdate(float deltaTime)
        {
            new Move
            {
                DataList = _data,
                DeltaTime = deltaTime
            }.Run(_data.Length);
        }

        public void AddData(IDataProvider item) { throw new System.NotImplementedException(); }

        public void RemoveData(int runtimeId) { throw new System.NotImplementedException(); }
    }
}