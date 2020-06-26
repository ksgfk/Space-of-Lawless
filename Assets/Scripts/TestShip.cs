using UnityEngine;
using UnityEngine.InputSystem;

namespace KSGFK
{
    public class TestShip : MonoBehaviour
    {
        private void Awake()
        {
            
        }

        private void Update()
        {
            var ship = GetComponent<CharacterController2D>();
            if (Keyboard.current.wKey.IsPressed())
            {
                ship.Move(Vector2.up * Time.deltaTime);
            }

            if (Keyboard.current.sKey.IsPressed())
            {
                ship.Move(Vector2.down * Time.deltaTime);
            }

            if (Keyboard.current.aKey.IsPressed())
            {
                ship.Move(Vector2.left * Time.deltaTime);
            }

            if (Keyboard.current.dKey.IsPressed())
            {
                ship.Move(Vector2.right * Time.deltaTime);
            }

            // var rigid = GetComponent<Rigidbody2D>();
            // var v = Vector2.zero;
            // if (Keyboard.current.wKey.IsPressed())
            // {
            //     v.y = 1;
            // }
            //
            // if (Keyboard.current.sKey.IsPressed())
            // {
            //     v.y = -1;
            // }
            //
            // rigid.velocity = v;
        }
    }
}