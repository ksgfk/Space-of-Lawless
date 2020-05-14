using UnityEngine;
using UnityEngine.InputSystem;

namespace KSGFK
{
    public class JobRotateProxy : MonoBehaviour, IJobCallback
    {
        public int id;
        public Transform target;

        public int DataId { get => id; set => id = value; }
        public IJobWrapper JobWrapper { get; set; }

        public void OnUpdate()
        {
            var rot = (JobRotate) JobWrapper;
            ref var data = ref rot[DataId];
            target.rotation = data.Rotation;
            data.Rotation = target.rotation;
        }

        public void OnInputCallback(InputAction.CallbackContext ctx)
        {
            var rot = (JobRotate) JobWrapper;
            ref var data = ref rot[DataId];
            data.Delta = ctx.ReadValue<Vector2>();
        }
    }
}