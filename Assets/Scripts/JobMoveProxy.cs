using UnityEngine;
using UnityEngine.InputSystem;

namespace KSGFK
{
    public class JobMoveProxy : MonoBehaviour, IJobCallback
    {
        public int id;
        public Transform target;

        public int DataId { get => id; set => id = value; }

        public IJobWrapper JobWrapper { get; set; }

        public void OnUpdate()
        {
            var move = (JobMove) JobWrapper;
            ref var moveData = ref move[DataId];
            var translate = new Vector3(moveData.Translation.x, moveData.Translation.y);
            target.Translate(translate);
        }

        public void OnInputCallback(InputAction.CallbackContext ctx)
        {
            var m = (JobMove) JobWrapper;
            ref var moveData = ref m[DataId];
            moveData.Direction = ctx.ReadValue<Vector2>();
        }
    }
}