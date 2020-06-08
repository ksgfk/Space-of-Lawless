using MPipeline;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace KSGFK
{
    public class JobMoveForTransform : JobTemplateForTransform<DataMoveWithTrans>
    {
        [BurstCompile]
        private struct MoveWithTransform : IJobParallelForTransform
        {
            public NativeList<DataMoveWithTrans> DataList;
            public float DeltaTime;

            public void Execute(int index, TransformAccess transform)
            {
                ref var data = ref DataList[index];
                var trans = math.normalizesafe(data.Direction) * data.Speed * DeltaTime;
                transform.position += new Vector3(trans.x, trans.y);
            }
        }

        public JobMoveForTransform(string name, int size) : base(name, size) { }

        public override void OnUpdate(float deltaTime)
        {
            var handler = new MoveWithTransform
            {
                DataList = DataArr,
                DeltaTime = deltaTime
            }.Schedule(TransArr);
            handler.Complete();
        }
    }
}