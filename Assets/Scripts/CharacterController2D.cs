using UnityEngine;

namespace KSGFK
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class CharacterController2D : MonoBehaviour
    {
        public LayerMask collideMask;
        [Range(0.1f, 15)] public float rayLength = 1;
        [Range(2, 10)] public int rayCount = 2;
        [Range(0, 25)] public float maxVelocity = 2;
        public float acceleration = 5;
        [Range(0, 100)] public float dumpForce = 1;

        private Rigidbody2D _rigid;
        private Collider2D _coll;
        [SerializeField] private Vector2 _velocity;
        private bool _isSpeedUp;

        public Vector2 Velocity => _velocity;

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
            _coll = GetComponent<Collider2D>();
            _rigid.bodyType = RigidbodyType2D.Kinematic;
            _rigid.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rigid.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void LateUpdate()
        {
            var fixedTime = Time.deltaTime;
            if (fixedTime.IsZero())
            {
                return;
            }

            var move = _velocity * fixedTime;
            MoveDetect(ref move);
            transform.position += (Vector3) move;
            if (_isSpeedUp)
            {
                _velocity = move / fixedTime;
                _isSpeedUp = false;
            }
            else
            {
                _velocity = Vector2.Lerp(move / fixedTime, Vector2.zero, fixedTime * dumpForce);
            }

            MathExt.SetSmallValZero(ref _velocity);
        }

        public void Move(Vector2 direction)
        {
            var deltaTime = Time.deltaTime;
            var tryAddV = _velocity + direction * (acceleration * deltaTime);
            if (tryAddV.magnitude > maxVelocity)
            {
                _velocity = tryAddV.normalized * maxVelocity;
            }
            else
            {
                _velocity = tryAddV;
            }

            _isSpeedUp = true;
        }

        private unsafe void MoveDetect(ref Vector2 deltaMove)
        {
            var ray = stackalloc RaycastHit2D[rayCount * 2];
            var bounds = _coll.bounds;
            var min = bounds.min;
            var max = bounds.max;
            var aabb = new Vector4(min.x, max.y, max.x, min.y);
            RayCastHorizontal(in aabb, in deltaMove, ray, out var horDir);
            RayCastVertical(in aabb, in deltaMove, ray, out var verDir);
            var trans = transform;
            for (var i = 0; i < rayCount * 2; i++)
            {
                var result = ray + i;
                if (!result->collider)
                {
                    continue;
                }

                if (result->transform == trans)
                {
                    continue;
                }

                var part = i < rayCount;
                var dis = Mathf.Abs(part ? deltaMove.x : deltaMove.y);
                var rayDis = result->distance;
                if (rayDis > dis)
                {
                    continue;
                }

                var newT = rayDis - 0.001f;
                if (part)
                {
                    deltaMove.x = newT * horDir.x;
                }
                else
                {
                    deltaMove.y = newT * verDir.y;
                }
            }
        }

        private unsafe void RayCastHorizontal(in Vector4 aabb, in Vector2 deltaMove, RaycastHit2D* arr, out Vector2 dir)
        {
            if (deltaMove.x.IsZero())
            {
                dir = Vector2.zero;
                return;
            }

            var isLeft = deltaMove.x < 0;
            var startX = isLeft ? aabb.x : aabb.z;
            dir = isLeft ? Vector2.left : Vector2.right;
            var perRayInterval = (aabb.y - aabb.w) / (rayCount - 1);
            for (var i = 0; i < rayCount; i++)
            {
                var startPos = new Vector2(startX, aabb.w + perRayInterval * i);
                arr[i] = Physics2D.Raycast(startPos, dir, rayLength, collideMask);
                Helper.DrawRay(startPos, dir * rayLength, Color.red);
            }
        }

        private unsafe void RayCastVertical(in Vector4 aabb, in Vector2 deltaMove, RaycastHit2D* arr, out Vector2 dir)
        {
            if (deltaMove.y.IsZero())
            {
                dir = Vector2.zero;
                return;
            }

            var isDown = deltaMove.y < 0;
            dir = isDown ? Vector2.down : Vector2.up;
            var startY = isDown ? aabb.w : aabb.y;
            var perRayInterval = (aabb.z - aabb.x) / (rayCount - 1);
            for (var i = 0; i < rayCount; i++)
            {
                var startPos = new Vector2(aabb.x + perRayInterval * i, startY);
                arr[i + rayCount] = Physics2D.Raycast(startPos, dir, rayLength, collideMask);
                Helper.DrawRay(startPos, dir * rayLength, Color.red);
            }
        }
    }
}