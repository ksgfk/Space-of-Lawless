using Unity.Mathematics;
using UnityEngine;

namespace KSGFK
{
    public class ShipModuleEngineJob : ShipModuleEngine,
        IJobCallback<JobTemplate<DataMove>>,
        IJobCallback<JobTemplate<DataRotate>>
    {
        public string moveJobName = "DefaultMoveJob";
        public string rotateJobName = "DefaultRotateJob";
        [SerializeField] private int moveDataId = -1;
        [SerializeField] private int rotateDataId = -1;

        int IJobCallback<JobTemplate<DataMove>>.DataId { get => moveDataId; set => moveDataId = value; }
        int IJobCallback<JobTemplate<DataRotate>>.DataId { get => rotateDataId; set => rotateDataId = value; }
        JobTemplate<DataMove> IJobCallback<JobTemplate<DataMove>>.Job { get; set; }
        JobTemplate<DataRotate> IJobCallback<JobTemplate<DataRotate>>.Job { get; set; }

        public override void OnAddToShip() { SetupJob(); }

        public override void OnRemoveFromShip() { RemoveJob(); }

        private void SetupJob()
        {
            var mov = GameManager.Job.GetJob<JobTemplate<DataMove>>(moveJobName);
            var rot = GameManager.Job.GetJob<JobTemplate<DataRotate>>(rotateJobName);
            mov.AddData(new DataMove {Speed = MaxMoveSpeed}, this);
            rot.AddData(new DataRotate
                {
                    Speed = MaxMoveSpeed,
                    Rotation = quaternion.identity
                },
                this);
            CanMove = true;
        }

        private void RemoveJob()
        {
            var mov = (IJobCallback<JobTemplate<DataMove>>) this;
            mov.Job.RemoveData(this);
            var rot = (IJobCallback<JobTemplate<DataRotate>>) this;
            rot.Job.RemoveData(this);
            CanMove = false;
        }

        public override void SetMoveDirection(Vector2 direction)
        {
            ref var movData = ref Helper.GetDataFromJob<DataMove>(this);
            movData.Direction = direction;
        }

        public override void SetRotateDelta(Vector2 delta)
        {
            ref var rotData = ref Helper.GetDataFromJob<DataRotate>(this);
            rotData.Delta = delta;
        }

        public override void Move()
        {
            ref var rotData = ref Helper.GetDataFromJob<DataRotate>(this);
            BaseShip.transform.rotation = rotData.Rotation;
            // rotData.Speed = MaxRotateSpeed;
            ref var movData = ref Helper.GetDataFromJob<DataMove>(this);
            var translate = new Vector3(movData.Translation.x, movData.Translation.y);
            BaseShip.transform.Translate(translate);
            // movData.Speed = MaxMoveSpeed;
        }
    }
}