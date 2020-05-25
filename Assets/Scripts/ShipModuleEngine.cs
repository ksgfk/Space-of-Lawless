using Unity.Mathematics;
using UnityEngine;

namespace KSGFK
{
    public class ShipModuleEngine : ShipModule, IJobCallback<MoveData>, IJobCallback<RotateData>
    {
        public string moveJobName = "DefaultMoveJob";
        public string rotateJobName = "DefaultRotateJob";
        [SerializeField] private float maxMoveSpeed = 0;
        [SerializeField] private float maxRotateSpeed = 0;
        [SerializeField] private int moveDataId = -1;
        [SerializeField] private int rotateDataId = -1;

        int IJobCallback<MoveData>.DataId { get => moveDataId; set => moveDataId = value; }
        int IJobCallback<RotateData>.DataId { get => rotateDataId; set => rotateDataId = value; }
        JobTemplate<MoveData> IJobCallback<MoveData>.Job { get; set; }
        JobTemplate<RotateData> IJobCallback<RotateData>.Job { get; set; }
        public float MaxMoveSpeed => maxMoveSpeed;
        public float MaxRotateSpeed => maxRotateSpeed;

        void IJobCallback<RotateData>.JobUpdate(ref RotateData data, ref ActionBuffer buffer)
        {
            BaseShip.transform.rotation = data.Rotation;
            data.Speed = MaxRotateSpeed;
        }

        void IJobCallback<MoveData>.JobUpdate(ref MoveData data, ref ActionBuffer buffer)
        {
            var translate = new Vector3(data.Translation.x, data.Translation.y);
            BaseShip.transform.Translate(translate);
            data.Speed = MaxMoveSpeed;
        }

        public void Init(float maxMovSpeed, float maxRotSpeed)
        {
            maxMoveSpeed = maxMovSpeed;
            maxRotateSpeed = maxRotSpeed;
        }

        public override void OnAddToShip() { SetupJob(); }

        public override void OnRemoveFromShip() { RemoveJob(); }

        private void SetupJob()
        {
            var mov = GameManager.Job.GetJob<JobTemplate<MoveData>>(moveJobName);
            var rot = GameManager.Job.GetJob<JobTemplate<RotateData>>(rotateJobName);
            mov.AddData(new MoveData {Speed = maxMoveSpeed}, this);
            rot.AddData(new RotateData
                {
                    Speed = maxRotateSpeed,
                    Rotation = quaternion.identity
                },
                this);
        }

        private void RemoveJob()
        {
            var mov = (IJobCallback<MoveData>) this;
            mov.Job.RemoveData(this);
            var rot = (IJobCallback<RotateData>) this;
            rot.Job.RemoveData(this);
        }
    }
}