using Unity.Mathematics;
using UnityEngine;

namespace KSGFK
{
    public class ShipModuleEngineJob : ShipModuleEngine, IJobCallback<MoveData>, IJobCallback<RotateData>
    {
        public string moveJobName = "DefaultMoveJob";
        public string rotateJobName = "DefaultRotateJob";
        [SerializeField] private int moveDataId = -1;
        [SerializeField] private int rotateDataId = -1;

        int IJobCallback<MoveData>.DataId { get => moveDataId; set => moveDataId = value; }
        int IJobCallback<RotateData>.DataId { get => rotateDataId; set => rotateDataId = value; }
        IJobWrapper IJobCallback<MoveData>.Job { get; set; }
        IJobWrapper IJobCallback<RotateData>.Job { get; set; }

        void IJobCallback<RotateData>.JobUpdate(ref RotateData data, ref ActionBuffer buffer)
        {
            // BaseShip.transform.rotation = data.Rotation;
            // data.Speed = MaxRotateSpeed;
        }

        void IJobCallback<MoveData>.JobUpdate(ref MoveData data, ref ActionBuffer buffer)
        {
            // var translate = new Vector3(data.Translation.x, data.Translation.y);
            // BaseShip.transform.Translate(translate);
            // data.Speed = MaxMoveSpeed;
        }

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
            var mov = (IJobCallback<MoveData>) this;
            ((JobTemplate<MoveData>) mov.Job).RemoveData(this);
            var rot = (IJobCallback<RotateData>) this;
            ((JobTemplate<RotateData>) rot.Job).RemoveData(this);
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