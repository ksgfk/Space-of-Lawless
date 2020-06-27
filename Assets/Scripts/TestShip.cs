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

        private void Awake()
        {
            _cc2d = GetComponent<CharacterController2D>();
            _rigid = GetComponent<Rigidbody2D>();
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

            var deltaTime = Time.deltaTime;
            _cc2d.Move(MathExt.DumpMove(_rigid.velocity, in m, moveDump, deltaTime) * deltaTime);
        }
    }
}