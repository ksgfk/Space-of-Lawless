using UnityEngine;

namespace KSGFK
{
    /// <summary>
    /// TODO:WIP
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class CharacterController2D : MonoBehaviour
    {
        public LayerMask collideMask;
        [Range(0.1f, float.MaxValue)] public float rayLength = 1;
        [Range(2, 10)] public int rayCount = 2;
        [SerializeField] private Vector2 _velocity;

        private Rigidbody2D _rigid;
        private Collider2D _coll;
        private Vector2 _translation;

        /// <summary>
        /// 单位:格/秒
        /// </summary>
        public Vector2 Velocity => _velocity;

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
            _coll = GetComponent<Collider2D>();
            _rigid.bodyType = RigidbodyType2D.Kinematic;
            _rigid.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void LateUpdate()
        {
            MoveHorizontal();
            MoveVertical();
            transform.position += (Vector3) _translation;
            var deltaTime = Time.deltaTime;
            if (!deltaTime.IsZero())
            {
                _velocity = _translation / deltaTime;
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

                var point = ray.point;
                if (Mathf.Abs(startPos.x + _translation.x) > Mathf.Abs(point.x))
                {
                    _translation.x = point.x - startPos.x - 0.000001f * dir.x;
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

                var point = ray.point;
                if (Mathf.Abs(startPos.y + _translation.y) > Mathf.Abs(point.y))
                {
                    //如果不减去0.000001，射线发射处会与碰撞体边界重合，重合部分似乎，检测不到
                    _translation.y = point.y - startPos.y - 0.000001f * dir.y;
                }
            }
        }

        private void Update()
        {
            Move(Vector2.up * Time.deltaTime);
            // Move(Vector2.down * Time.deltaTime);
            Move(Vector2.left * Time.deltaTime);
            // Move(Vector2.right * Time.deltaTime);
        }
    }
}