using UnityEngine;

namespace KSGFK
{
    [DisallowMultipleComponent]
    public class EntityBulletBallistic : EntityBullet
    {
        private JobInfo<JobTranslateOutput> _jobInfo;

        public override void Launch(Vector2 direction, Vector2 startPos, float speed, float duration)
        {
            var trans = transform;
            trans.position = startPos;
            trans.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.up, direction, Vector3.forward),
                Vector3.forward);
            Jobs.Translate.AddValue(new JobTranslateInput
                {
                    Transform = trans,
                    Velocity = new Vector2(0, speed)
                },
                _jobInfo);
        }

        public override void OnSpawn()
        {
            if (_jobInfo == null)
            {
                _jobInfo = JobInfo<JobTranslateOutput>.Default;
            }
        }

        public override void OnRemoveFromWorld()
        {
            if (!_jobInfo.IsDefault)
            {
                Jobs.Translate.RemoveValue(_jobInfo);
            }
        }
    }
}