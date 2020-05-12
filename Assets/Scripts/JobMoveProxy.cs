using UnityEngine;

namespace KSGFK
{
    public class JobMoveProxy : MonoBehaviour, IDataProvider<MoveData>
    {
        public int id;
        public Transform moveTarget;

        public int DataId { get => id; set => id = value; }

        public IJobWrapper JobWrapper { get; set; }

        public void PostUpdate()
        {
            var move = (JobMove) JobWrapper;
            ref var moveData = ref move[DataId];
            var translate = new Vector3(moveData.Translation.x, moveData.Translation.y);
            moveTarget.Translate(translate);
        }
    }
}