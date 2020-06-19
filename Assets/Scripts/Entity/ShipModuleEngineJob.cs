using Unity.Mathematics;
using UnityEngine;

namespace KSGFK
{
    public class ShipModuleEngineJob : ShipModuleEngine
    {
        public string jobName = "MoveAndRotate";
        [SerializeField] private JobInfo jobInfo = JobInfo.Default;
        private JobWrapperImpl<JobMR4TInitReq, DataMoveRotate> _job;

        public override bool CanMove
        {
            get => canMove;
            set
            {
                if (value)
                {
                    jobInfo = _job.AddData(new JobMR4TInitReq
                    {
                        Direction = Vector2.zero,
                        MoveSpeed = MaxMoveSpeed,
                        RotateDelta = Vector2.zero,
                        Trans = BaseShip.transform
                    });
                }
                else
                {
                    _job.RemoveData(jobInfo);
                }

                canMove = value;
            }
        }

        public override void OnAddToShip() { SetupJob(); }

        public override void OnRemoveFromShip() { RemoveJob(); }

        private void SetupJob()
        {
            _job = GameManager.Job.GetJob<JobMR4TInitReq, DataMoveRotate>(jobName);
            CanMove = true;
        }

        private void RemoveJob() { CanMove = false; }

        public override void MoveDirection(Vector2 direction)
        {
            ref var moveData = ref _job[jobInfo];
            moveData.Direction = direction;
        }

        public override void RotateDelta(Vector2 delta)
        {
            ref var rotateData = ref _job[jobInfo];
            rotateData.RotateDelta = delta;
        }

        public override void Rotate(float angle)
        {
            ref var rotateData = ref _job[jobInfo];
            math.sincos(angle, out var sinA, out var cosA);
            rotateData.RotateDelta = new float2(cosA, sinA);
        }
    }
}