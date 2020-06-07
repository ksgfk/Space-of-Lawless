using UnityEngine;

namespace KSGFK
{
    public class EntityBulletBallistic : EntityBullet, IJobCallback<JobMoveWithTransform>
    {
        public string moveJobName = "DefaultMoveWithTrans";
        [SerializeField] private int jobDataId = -1;
        [SerializeField] private float expireTime;
        private JobMoveWithTransform _moveJob;

        public int DataId { get => jobDataId; set => jobDataId = value; }
        JobMoveWithTransform IJobCallback<JobMoveWithTransform>.Job { get => _moveJob; set => _moveJob = value; }

        private void Update()
        {
            if (Time.time >= expireTime)
            {
                GameManager.Entity.DestroyEntity(this);
            }
        }

        public override void Launch(Vector2 direction, Vector2 startPos, float speed, float duration)
        {
            var data = new DataMoveWithTrans
            {
                Direction = direction,
                Speed = speed
            };
            _moveJob.AddData(transform, data, this);
            expireTime = Time.time + duration;
            var trans = transform;
            trans.rotation = MathExt.FromToRotation(Vector3.up, direction);
            trans.position = startPos;
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            _moveJob = GameManager.Job.GetJob<JobMoveWithTransform>(moveJobName);
        }

        public override void OnRemoveFromWorld()
        {
            base.OnRemoveFromWorld();
            _moveJob.RemoveData(this);
        }
    }
}