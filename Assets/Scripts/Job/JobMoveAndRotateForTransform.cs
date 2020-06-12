using MPipeline;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace KSGFK
{
    public struct JobMR4TInitReq
    {
        public Transform Trans;
        public float MoveSpeed;
        public Vector2 Direction;
        public Vector2 RotateDelta;
    }

    public struct DataMoveRotate
    {
        public float MoveSpeed;
        public Vector2 Direction;
        public Vector2 RotateDelta;
    }

    public class JobMoveAndRotateForTransform : JobWrapperImpl<JobMR4TInitReq, DataMoveRotate>
    {
        private TransformAccessArray _transArr;

        [BurstCompile]
        private struct MoveAndRotateForTransform : IJobParallelForTransform
        {
            public NativeList<DataMoveRotate> DataList;
            public float DeltaTime;

            public void Execute(int index, TransformAccess transform)
            {
                ref var data = ref DataList[index];
                var translation = math.normalizesafe(data.Direction) * data.MoveSpeed * DeltaTime;
                var rotation = MathExt.FromToRotation(new float3(0, 1, 0), new float3(data.RotateDelta, 0));
                transform.position += (Vector3) MathExt.TransformDirection(rotation, new float3(translation, 0));
                transform.rotation = rotation;
            }
        }

        public JobMoveAndRotateForTransform() { _transArr = new TransformAccessArray(0, 2); }

        public override void OnUpdate()
        {
            var handle = new MoveAndRotateForTransform
            {
                DataList = DataList,
                DeltaTime = Time.deltaTime
            }.Schedule(_transArr);
            handle.Complete();
        }

        protected override void OnAddData(ref JobMR4TInitReq data)
        {
            DataList.Add(new DataMoveRotate
            {
                RotateDelta = data.RotateDelta,
                Direction = data.Direction,
                MoveSpeed = data.MoveSpeed
            });
            _transArr.Add(data.Trans);
        }

        protected override void OnRemoveData(int id)
        {
            base.OnRemoveData(id);
            _transArr.RemoveAtSwapBack(id);
        }

        public override void Dispose()
        {
            base.Dispose();
            _transArr.Dispose();
        }
    }
}