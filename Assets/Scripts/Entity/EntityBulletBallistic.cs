using UnityEngine;

namespace KSGFK
{
    [DisallowMultipleComponent]
    public class EntityBulletBallistic : EntityBullet
    {
        private JobInfo _transInfo;
        private JobInfo _rmInfo;

        public override void Launch(Vector2 direction, Vector2 startPos, float speed, float duration)
        {
            var trans = transform;
            trans.position = startPos;
            Jobs.RotateTemp.AddValue(new JobRotateInput
            {
                Direction = direction,
                Standard = Vector2.up,
                Trans = trans
            });
            Jobs.TranslatePersist.AddValue(new JobTranslateInput
                {
                    Transform = trans,
                    Velocity = new Vector2(0, speed)
                },
                _transInfo);
            Jobs.TimingTask.AddTask(duration, () => GameManager.Instance.World.Value.DestroyEntity(this), _rmInfo);
        }

        public override void OnSpawn()
        {
            if (_transInfo == null)
            {
                _transInfo = JobInfo.Default;
            }

            if (_rmInfo == null)
            {
                _rmInfo = JobInfo.Default;
            }
        }

        public override void OnRemoveFromWorld()
        {
            if (!_transInfo.IsDefault)
            {
                Jobs.TranslatePersist.RemoveValue(_transInfo);
            }

            if (!_rmInfo.IsDefault)
            {
                Jobs.TimingTask.RemoveTask(_rmInfo);
            }
        }
    }
}