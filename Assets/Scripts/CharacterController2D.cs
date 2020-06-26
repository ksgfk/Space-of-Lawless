using UnityEngine;

namespace KSGFK
{
    /// <summary>
    /// TODO:RaycastCommand
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class CharacterController2D : MonoBehaviour
    {
        public LayerMask collideMask;
        [Range(0.1f, float.MaxValue)] public float rayLength = 1;
        [Range(2, 10)] public int rayCount = 2;
        public float minSpeed = 0.00001f;

        private Rigidbody2D _rigid;
        private Collider2D _coll;
        [SerializeField] private Vector2 _translation;

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
            _coll = GetComponent<Collider2D>();
            _rigid.bodyType = RigidbodyType2D.Kinematic;
            _rigid.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rigid.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void FixedUpdate()
        {
            MoveHorizontal();
            MoveVertical();
            var fixedTime = Time.fixedDeltaTime;
            if (!fixedTime.IsZero())
            {
                var velocity = _translation / fixedTime;
                if (Mathf.Abs(velocity.x) < minSpeed)
                {
                    velocity.x = 0f;
                }

                if (Mathf.Abs(velocity.y) < minSpeed)
                {
                    velocity.y = 0f;
                }

                _rigid.velocity = velocity;
            }

            _translation = Vector2.zero;
        }

        public void Move(Vector2 translation) { _translation += translation; }

        private void MoveHorizontal()
        {
            if (_translation.x.IsZero())
            {
                return;
            }

            var border = _coll.bounds;
            var leftUp = new Vector2(border.min.x, border.max.y);
            var rightDown = new Vector2(border.max.x, border.min.y);
            var isLeft = _translation.x < 0;
            var startX = isLeft ? leftUp.x : rightDown.x;
            var dir = isLeft ? Vector2.left : Vector2.right;
            var perRayInterval = (leftUp.y - rightDown.y) / (rayCount - 1);
            for (var i = 0; i < rayCount; i++)
            {
                var startPos = new Vector2(startX, rightDown.y + perRayInterval * i);
                var ray = Physics2D.Raycast(startPos, dir, rayLength, collideMask);
                Helper.DrawRay(startPos, dir * rayLength, Color.red);
                if (!ray)
                {
                    continue;
                }

                if (ray.distance <= Mathf.Abs(_translation.x))
                {
                    _translation.x = (ray.distance - 0.001f) * dir.x;
                }
            }
        }

        private void MoveVertical()
        {
            if (_translation.y.IsZero())
            {
                return;
            }

            var border = _coll.bounds;
            var leftUp = new Vector2(border.min.x, border.max.y);
            var rightDown = new Vector2(border.max.x, border.min.y);
            var isDown = _translation.y < 0;
            var dir = isDown ? Vector2.down : Vector2.up;
            var startY = isDown ? rightDown.y : leftUp.y;
            var perRayInterval = (rightDown.x - leftUp.x) / (rayCount - 1);
            for (var i = 0; i < rayCount; i++)
            {
                var startPos = new Vector2(leftUp.x + perRayInterval * i, startY);
                var ray = Physics2D.Raycast(startPos, dir, rayLength, collideMask);
                Helper.DrawRay(startPos, dir * rayLength, Color.red);
                if (!ray)
                {
                    continue;
                }

                if (ray.distance <= Mathf.Abs(_translation.y))
                {
                    _translation.y = (ray.distance - 0.001f) * dir.y;
                }
            }
        }
    }
}