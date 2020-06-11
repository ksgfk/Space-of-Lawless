using Unity.Mathematics;
using UnityEngine;

namespace KSGFK
{
    public class ShipModuleEngineJob : ShipModuleEngine
    {
        public string moveJobName = "DefaultMoveJob";
        public string rotateJobName = "DefaultRotateJob";
        [SerializeField] private JobInfo moveInfo = JobInfo.Default;
        [SerializeField] private JobInfo rotateInfo = JobInfo.Default;
        private JobWrapperImpl<JobMoveInitReq, DataMove> _moveJob;

        public override void OnAddToShip() { SetupJob(); }

        public override void OnRemoveFromShip() { RemoveJob(); }

        private void SetupJob()
        {
            _moveJob = GameManager.Job.GetJob<JobMoveInitReq, DataMove>(moveJobName);
            moveInfo = _moveJob.AddData(new JobMoveInitReq {Direction = new float2(0, 1), Speed = MaxMoveSpeed});
            // var rot = GameManager.Job.GetJob(rotateJobName);
            // mov.AddData(new DataMove {Speed = MaxMoveSpeed}, this);
            // rot.AddData(new DataRotate
            //     {
            //         Speed = MaxMoveSpeed,
            //         Rotation = quaternion.identity
            //     },
            //     this);
            CanMove = true;
        }

        private void RemoveJob()
        {
            _moveJob.RemoveData(moveInfo);
            CanMove = false;
            // var mov = (IJobCallback<JobTemplate<DataMove>>) this;
            // mov.Job.RemoveData(this);
            // var rot = (IJobCallback<JobTemplate<DataRotate>>) this;
            // rot.Job.RemoveData(this);
            // CanMove = false;
        }

        public override void SetMoveDirection(Vector2 direction)
        {
            ref var moveData = ref _moveJob[moveInfo];
            moveData.Direction = direction;
        }

        public override void SetRotateDelta(Vector2 delta)
        {
            // ref var rotData = ref Helper.GetDataFromJob<DataRotate>(this);
            // rotData.Delta = delta;
        }

        public override void Move()
        {
            ref var moveData = ref _moveJob[moveInfo];
            ref var trans = ref moveData.Translation;
            var translate = new Vector3(trans.x, trans.y);
            BaseShip.transform.position += translate;
            // ref var rotData = ref Helper.GetDataFromJob<DataRotate>(this);
            // BaseShip.transform.rotation = rotData.Rotation;
            // // rotData.Speed = MaxRotateSpeed;
            // ref var movData = ref Helper.GetDataFromJob<DataMove>(this);
            // var translate = new Vector3(movData.Translation.x, movData.Translation.y);
            // BaseShip.transform.Translate(translate);
            // // movData.Speed = MaxMoveSpeed;
        }
    }
}