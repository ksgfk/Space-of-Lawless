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
            var pos = transform.position;
            var border = _coll.bounds;
            var topLeft = new Vector2(border.min.x, border.max.y);
            var bottomRight = new Vector2(border.max.x, border.min.y);
            Vector2 startPos;
            Vector2 dir;
            if (!translation.x.IsZero())
            {
                var isLeft = translation.x < 0;
                var startX = isLeft ? topLeft.x : bottomRight.x;
                dir = isLeft ? Vector2.left : Vector2.right;
                startPos = new Vector2(startX, pos.y);
            }
            else if (!translation.y.IsZero())
            {
                var isBottom = translation.y < 0;
                var startY = isBottom ? bottomRight.y : topLeft.y;
                dir = isBottom ? Vector2.down : Vector2.up;
                startPos = new Vector2(pos.x, startY);
            }
            else
            {
                return;
            }

            var ray = Physics2D.Raycast(startPos, dir, rayLength, collideMask);
            Debug.DrawLine(startPos, startPos + dir * rayLength, Color.red);
        }

        private void Update()
        {
            Move(Vector2.up * Time.deltaTime);
            Move(Vector2.down * Time.deltaTime);
            Move(Vector2.left * Time.deltaTime);
            Move(Vector2.right * Time.deltaTime);
        }
    }
}