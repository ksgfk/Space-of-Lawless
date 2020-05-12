using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KSGFK
{
    public unsafe class JobMoveProxy : MonoBehaviour, IDataCallback
    {
        public int id;
        public Transform moveTarget;

        public int DataId { get => id; set => id = value; }

        public IJobWrapper JobWrapper { get; set; }
        
        public void OnUpdate()
        {
            var move = (JobMove) JobWrapper;
            ref var moveData = ref move[DataId];
            var translate = new Vector3(moveData.Translation.x, moveData.Translation.y);
            moveTarget.Translate(translate);
        }

        public void OnInputCallback(InputAction.CallbackContext ctx)
        {
            var m = (JobMove) JobWrapper;
            ref var moveData = ref m[DataId];
            moveData.Direction = ctx.ReadValue<Vector2>();
        }
    }
}