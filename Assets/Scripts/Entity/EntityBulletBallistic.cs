using UnityEngine;

namespace KSGFK
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [DisallowMultipleComponent]
    public class EntityBulletBallistic : EntityBullet
    {
        public LayerMask collideLayer;

        private JobInfo _transInfo;
        private JobInfo _rmInfo;

        protected override void Launch(Vector2 direction, Vector2 startPos, float speed)
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!MathExt.ContainsLayer(other.gameObject.layer, collideLayer))
            {
                return;
            }

            OnTriggerCollider(other.gameObject);
        }

        protected virtual void OnTriggerCollider(GameObject go)
        {
            if (go == _launcher.gameObject)
            {
                return;
            }

            GameManager.Instance.World.Value.DestroyEntity(this);
        }
    }
}