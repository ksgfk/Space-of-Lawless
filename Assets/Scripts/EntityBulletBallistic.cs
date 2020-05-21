using Unity.Mathematics;
using UnityEngine;

namespace KSGFK
{
    public class EntityBulletBallistic : EntityBullet, IJobCallback<MoveData>
    {
        public string moveJobName = "DefaultMoveJob";
        [SerializeField] private int jobDataId;
        [SerializeField] private float expireTime;
        private JobTemplate<MoveData> _moveJob;

        public int DataId { get => jobDataId; set => jobDataId = value; }

        public void OnUpdate(ref MoveData data)
        {
            ref var trans = ref data.Translation;
            transform.Translate(new Vector3(trans.x, trans.y));
            if (Time.time >= expireTime)
            {
                GameManager.Entity.DestroyEntityLate(this);
            }
        }

        public override void Launch(Vector2 direction, Vector2 startPos, float speed, float duration)
        {
            _moveJob.AddData(new MoveData
                {
                    Direction = new float2(0, 1),
                    Speed = speed
                },
                this);
            expireTime = Time.time + duration;
            var trans = transform;
            trans.rotation = MathExt.FromToRotation(Vector3.up, direction);
            trans.position = startPos;
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            _moveJob = GameManager.Job.GetJob<JobTemplate<MoveData>>(moveJobName);
        }

        public override void OnRemoveFromWorld()
        {
            base.OnRemoveFromWorld();
            _moveJob.RemoveData(this);
        }
    }
}