using UnityEngine;
using UnityEngine.InputSystem;

namespace KSGFK
{
    public class TestShip : MonoBehaviour
    {
        public float moveSpeed;
        public float moveDump;

        private CharacterController2D _cc2d;
        private Rigidbody2D _rigid;
        private Camera _cam;

        private void Awake()
        {
            _cc2d = GetComponent<CharacterController2D>();
            _rigid = GetComponent<Rigidbody2D>();
            _cam = Camera.main;
        }

        private void Update()
        {
            var m = Vector2.zero;
            if (Keyboard.current.wKey.IsPressed())
            {
                m.y = 1;
            }

            if (Keyboard.current.sKey.IsPressed())
            {
                m.y = -1;
            }

            if (Keyboard.current.aKey.IsPressed())
            {
                m.x = -1;
            }

            if (Keyboard.current.dKey.IsPressed())
            {
                m.x = 1;
            }

            var worldPos = (Vector2) _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var r = MathExt.FromToRotation(Vector3.up, worldPos - (Vector2) transform.position);
            var deltaTime = Time.deltaTime;
            _cc2d.Move(MathExt.DumpMove(_rigid.velocity, r * m * moveSpeed, moveDump, deltaTime) * deltaTime);
        }
    }
}