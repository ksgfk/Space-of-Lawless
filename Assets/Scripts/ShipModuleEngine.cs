using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KSGFK
{
    public class ShipModuleEngine : ShipModule, IJobCallback<MoveData>, IJobCallback<RotateData>
    {
        [SerializeField] private float maxMoveSpeed;

        [SerializeField] private float maxRotateSpeed;
        [SerializeField] private int moveDataId;
        [SerializeField] private int rotateDataId;
        private JobTemplate<MoveData> _mov;
        private JobTemplate<RotateData> _rot;

        int IJobCallback<MoveData>.DataId { get => moveDataId; set => moveDataId = value; }
        int IJobCallback<RotateData>.DataId { get => rotateDataId; set => rotateDataId = value; }
        public float MaxMoveSpeed => maxMoveSpeed;
        public float MaxRotateSpeed => maxRotateSpeed;

        void IJobCallback<RotateData>.OnUpdate(ref RotateData data) { BaseShip.transform.rotation = data.Rotation; }

        void IJobCallback<MoveData>.OnUpdate(ref MoveData data)
        {
            var translate = new Vector3(data.Translation.x, data.Translation.y);
            BaseShip.transform.Translate(translate);
        }

        public void SetupJob(JobTemplate<MoveData> mov, JobTemplate<RotateData> rot)
        {
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