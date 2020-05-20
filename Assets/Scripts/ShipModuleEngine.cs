using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

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
        private JobTemplate<MoveData> _mov = null;
        private JobTemplate<RotateData> _rot = null;

        int IJobCallback<MoveData>.DataId { get => moveDataId; set => moveDataId = value; }
        int IJobCallback<RotateData>.DataId { get => rotateDataId; set => rotateDataId = value; }
        public float MaxMoveSpeed => maxMoveSpeed;
        public float MaxRotateSpeed => maxRotateSpeed;

        void IJobCallback<RotateData>.OnUpdate(ref RotateData data)
        {
            BaseShip.transform.rotation = data.Rotation;
            data.Speed = MaxRotateSpeed;
        }

        void IJobCallback<MoveData>.OnUpdate(ref MoveData data)
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

        private void SetupJob()
        {
            var mov = GameManager.Job.GetJob<JobTemplate<MoveData>>(moveJobName);
            var rot = GameManager.Job.GetJob<JobTemplate<RotateData>>(rotateJobName);
            _mov = mov;
            _rot = rot;
            mov.AddData(new MoveData {Speed = maxMoveSpeed}, this);
            rot.AddData(new RotateData
                {
                    Speed = maxRotateSpeed,
                    Rotation = quaternion.identity
                },
                this);
        }

        public void OnInputCallbackMove(InputAction.CallbackContext ctx)
        {
            ref var movData = ref _mov[moveDataId];
            movData.Direction = ctx.ReadValue<Vector2>();
        }

        public void OnInputCallbackRotate(InputAction.CallbackContext ctx)
        {
            ref var rotData = ref _rot[rotateDataId];
            rotData.Delta = ctx.ReadValue<Vector2>();
        }
    }
}