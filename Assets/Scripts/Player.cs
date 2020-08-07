using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KSGFK
{
    public class Player : MonoBehaviour
    {
        public Entity player;
        public CharacterController2D cc2d;
        public Vector2 pointerPos;
        public Vector2 moveDir;

        private EntityLiving _living;

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
            ctrl.Player.Point.performed -= MousePos;
            ctrl.Player.Move.performed -= StartMove;
            ctrl.Player.Move.canceled -= StopMove;
            ctrl.Player.Action.performed -= StartAct;
            ctrl.Player.Action.canceled -= StopAct;
        }

        public void Setup(Entity entity)
        {
            player = entity;
            var ctrl = GameManager.Instance.Input;
            if (player.TryGetComponent(out cc2d))
            {
                ctrl.Player.Point.performed += MousePos;
                ctrl.Player.Move.performed += StartMove;
                ctrl.Player.Move.canceled += StopMove;
            }
            else
            {
                Debug.Log($"{entity.name}没有CC2D");
                player = null;
            }

            if (player is EntityLiving living)
            {
                _living = living;
                var hasInv = _living.Inventory;
                if (hasInv.HasValue)
                {
                    ctrl.Player.Action.performed += StartAct;
                    ctrl.Player.Action.canceled += StopAct;
                }
            }
        }

        private void MousePos(InputAction.CallbackContext ctx) { pointerPos = ctx.ReadValue<Vector2>(); }

        private void StartMove(InputAction.CallbackContext ctx) { moveDir = ctx.ReadValue<Vector2>(); }

        private void StopMove(InputAction.CallbackContext ctx) { moveDir = Vector2.zero; }

        private void StartAct(InputAction.CallbackContext ctx)
        {
            Inventory inv = _living.Inventory;
            inv.PickRadiusItems();
        }

        private void StopAct(InputAction.CallbackContext ctx) { }
    }
}