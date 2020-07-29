using UnityEngine;
using UnityEngine.InputSystem;
using InputCtx = System.Action<UnityEngine.InputSystem.InputAction.CallbackContext>;

namespace KSGFK
{
    public class Player : MonoBehaviour
    {
        public Entity player;
        public CharacterController2D cc2d;
        public Vector2 pointerPos;
        public Vector2 moveDir;

        private InputCtx _mouseMove;
        private InputCtx _playerStartMove;
        private InputCtx _playerStopMove;

        private void Update()
        {
            if (moveDir != Vector2.zero)
            {
                Vector2 targetPos = GameManager.Instance.MainCamera.ScreenToWorldPoint(pointerPos);
                Vector2 nowPos = player.transform.position;
                var dir = (targetPos - nowPos).normalized;
                var result = MathExt.ConvertCoord(dir, moveDir);
                cc2d.Move(result);
            }
        }

        private void OnDestroy()
        {
            var ctrl = GameManager.Instance.Input;
            if (_mouseMove != null)
            {
                ctrl.Player.Point.performed -= _mouseMove;
            }

            if (_playerStartMove != null)
            {
                ctrl.Player.Move.performed -= _playerStartMove;
            }

            if (_playerStopMove != null)
            {
                ctrl.Player.Move.canceled -= _playerStopMove;
            }
        }

        public void Setup(Entity entity)
        {
            player = entity;
            if (player.TryGetComponent(out cc2d))
            {
                var ctrl = GameManager.Instance.Input;
                _mouseMove = MousePos;
                _playerStartMove = StartMove;
                _playerStopMove = StopMove;
                ctrl.Player.Point.performed += _mouseMove;
                ctrl.Player.Move.performed += _playerStartMove;
                ctrl.Player.Move.canceled += _playerStopMove;
            }
            else
            {
                Debug.Log($"{entity.name}没有CC2D");
                player = null;
            }
        }

        private void MousePos(InputAction.CallbackContext ctx) { pointerPos = ctx.ReadValue<Vector2>(); }

        private void StartMove(InputAction.CallbackContext ctx) { moveDir = ctx.ReadValue<Vector2>(); }

        private void StopMove(InputAction.CallbackContext ctx) { moveDir = Vector2.zero; }
    }
}