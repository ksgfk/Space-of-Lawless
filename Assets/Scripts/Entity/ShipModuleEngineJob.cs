using Unity.Mathematics;
using UnityEngine;

namespace KSGFK
{
    public class ShipModuleEngineJob : ShipModuleEngine,
        IJobCallback<JobTemplate<MoveData>>,
        IJobCallback<JobTemplate<RotateData>>
    {
        public string moveJobName = "DefaultMoveJob";
        public string rotateJobName = "DefaultRotateJob";
        [SerializeField] private int moveDataId = -1;
        [SerializeField] private int rotateDataId = -1;

        int IJobCallback<JobTemplate<MoveData>>.DataId { get => moveDataId; set => moveDataId = value; }
        int IJobCallback<JobTemplate<RotateData>>.DataId { get => rotateDataId; set => rotateDataId = value; }
        JobTemplate<MoveData> IJobCallback<JobTemplate<MoveData>>.Job { get; set; }
        JobTemplate<RotateData> IJobCallback<JobTemplate<RotateData>>.Job { get; set; }

        public override void OnAddToShip() { SetupJob(); }

        public override void OnRemoveFromShip() { RemoveJob(); }

        private void SetupJob()
        {
            var mov = GameManager.Job.GetJob<JobTemplate<MoveData>>(moveJobName);
            var rot = GameManager.Job.GetJob<JobTemplate<RotateData>>(rotateJobName);
            mov.AddData(new MoveData {Speed = MaxMoveSpeed}, this);
            rot.AddData(new RotateData
                {
                    Speed = MaxMoveSpeed,
                    Rotation = quaternion.identity
                },
                this);
            CanMove = true;
        }

        private void RemoveJob()
        {
            var mov = (IJobCallback<JobTemplate<MoveData>>) this;
            mov.Job.RemoveData(this);
            var rot = (IJobCallback<JobTemplate<RotateData>>) this;
            rot.Job.RemoveData(this);
            CanMove = false;
        }

        public override void SetMoveDirection(Vector2 direction)
        {
            ref var movData = ref Helper.GetDataFromJob<MoveData>(this);
            movData.Direction = direction;
        }

        public override void SetRotateDelta(Vector2 delta)
        {
            ref var rotData = ref Helper.GetDataFromJob<RotateData>(this);
            rotData.Delta = delta;
        }

        public override void Move()
        {
            ref var rotData = ref Helper.GetDataFromJob<RotateData>(this);
            BaseShip.transform.rotation = rotData.Rotation;
            rotData.Speed = MaxRotateSpeed;
            ref var movData = ref Helper.GetDataFromJob<MoveData>(this);
            var translate = new Vector3(movData.Translation.x, movData.Translation.y);
            BaseShip.transform.Translate(translate);
            movData.Speed = MaxMoveSpeed;
        }
    }
}