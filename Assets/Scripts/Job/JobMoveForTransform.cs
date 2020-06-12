using MPipeline;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace KSGFK
{
    public struct JobMoveForTransformInitReq
    {
        public float Speed;
        public Vector2 Direction;
        public Transform Trans;
    }

    public struct DataMoveForTransform
    {
        public float Speed;
        public float2 Direction;
    }

    public class JobMoveForTransform : JobWrapperImpl<JobMoveForTransformInitReq, DataMoveForTransform>
    {
        private TransformAccessArray _transList;

        [BurstCompile]
        private struct MoveWithTransform : IJobParallelForTransform
        {
            public NativeList<DataMoveForTransform> DataList;
            public float DeltaTime;

            public void Execute(int index, TransformAccess transform)
            {
                ref var data = ref DataList[index];
                var trans = math.normalizesafe(data.Direction) * data.Speed * DeltaTime;
                transform.position += new Vector3(trans.x, trans.y);
            }
        }

        public JobMoveForTransform() { _transList = new TransformAccessArray(0, 6); }

        public override void OnUpdate()
        {
            var handler = new MoveWithTransform
            {
                DataList = DataList,
                DeltaTime = Time.deltaTime
            }.Schedule(_transList);
            handler.Complete();
        }

        protected override void OnAddData(ref JobMoveForTransformInitReq data)
        {
            DataList.Add(new DataMoveForTransform {Direction = data.Direction, Speed = data.Speed});
            _transList.Add(data.Trans);
        }

        protected override void OnRemoveData(int id) { _transList.RemoveAtSwapBack(id); }

        public override void Dispose()
        {
            base.Dispose();
            _transList.Dispose();
        }

        protected override bool CheckLength() { return base.CheckLength() && DataList.Length == _transList.length; }
    }
}