using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KSGFK
{
    public class Ship : MonoBehaviour
    {
        public Vector2 move;
        public Vector2 look;
        private PlayerInput _input;

        private void Start()
        {
            _input = GetComponent<PlayerInput>();
            _input.onActionTriggered += ctx =>
            {
                switch (ctx.action.name)
                {
                    case "Move":
                        move = ctx.ReadValue<Vector2>();
                        break;
                    case "Point":
                        look = ctx.ReadValue<Vector2>();
                        break;
                }
            };
        }

        private void Update()
        {
            var trans = transform;
            var dir = move.normalized;
            dir *= Time.deltaTime;
            trans.Translate(dir);

            var target = Camera.main.ScreenToWorldPoint(look);
            var now = trans.position;
            float3 d = target - now;
            d.z = 0;
            var res = MathExt.FromToRotation(new float3(0, 1, 0), d);
            trans.rotation = math.slerp(trans.rotation, res, 0.5f * Time.deltaTime);
        }
    }
}