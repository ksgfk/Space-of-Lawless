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

        private Rigidbody2D _rigid;
        private Collider2D _coll;

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
            _coll = GetComponent<Collider2D>();
            _rigid.bodyType = RigidbodyType2D.Kinematic;
            _rigid.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        public void Move(Vector2 translation)
        {
            MoveHorizontal(ref translation);
            MoveVertical(ref translation);
            _rigid.transform.position += (Vector3) translation;
        }

        private void MoveHorizontal(ref Vector2 translation)
        {
            if (translation.x.IsZero())
            {
                return;
            }

            var border = _coll.bounds;
            var isLeft = translation.x < 0;
            var startX = isLeft ? border.min.x : border.max.x;
            var dir = isLeft ? Vector2.left : Vector2.right;
            var startPos = new Vector2(startX, transform.position.y);
        }

        private void MoveVertical(ref Vector2 translation)
        {
            if (translation.y.IsZero())
            {
                return;
            }

            var border = _coll.bounds;
            var leftUp = new Vector2(border.min.x, border.max.y);
            var rightDown = new Vector2(border.max.x, border.min.y);
            var isDown = translation.y < 0;
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
                if (Mathf.Abs(startPos.y + translation.y) > Mathf.Abs(point.y))
                {
                    //如果不减去0.000001，射线发射处会与碰撞体边界重合，重合部分似乎，检测不到
                    translation.y = point.y - startPos.y - 0.000001f * dir.y;
                }
            }
        }

        private void Update()
        {
            Move(Vector2.up * Time.deltaTime);
            // Move(Vector2.down * Time.deltaTime);
            // Move(Vector2.left * Time.deltaTime);
            // Move(Vector2.right * Time.deltaTime);
        }
    }
}