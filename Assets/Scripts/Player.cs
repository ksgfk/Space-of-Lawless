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
        public bool isUseItem;
        public int maxSlot = -1;
        public int nowSlot = -1;

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

            if (isUseItem)
            {
                Inventory inv = ((EntityLiving) player).Inventory;
                inv.UseHeldItem();
            }
        }

        private void OnDestroy()
        {
            var ctrl = GameManager.Instance.Input;
            ctrl.Player.Point.performed -= MousePos;
            ctrl.Player.Move.performed -= StartMove;
            ctrl.Player.Move.canceled -= StopMove;
            ctrl.Player.Act.performed -= Pickup;
            ctrl.Player.Select.performed -= SelectSlot;
            ctrl.Player.Fire.started -= StartUseItem;
            ctrl.Player.Fire.canceled -= StopUseItem;
            ctrl.Player.Point.performed -= RotateInv;
            ctrl.Player.Point.performed -= RotateBody;
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
                ctrl.Player.Point.performed += RotateBody;
            }
            else
            {
                Debug.Log($"{entity.name}没有CC2D");
                player = null;
            }

            if (player is EntityLiving living)
            {
                var hasInv = living.Inventory;
                if (hasInv.HasValue)
                {
                    var inv = hasInv.Value;
                    ctrl.Player.Act.performed += Pickup;
                    ctrl.Player.Select.performed += SelectSlot;
                    ctrl.Player.Fire.started += StartUseItem;
                    ctrl.Player.Fire.canceled += StopUseItem;
                    ctrl.Player.Point.performed += RotateInv;
                    maxSlot = inv.Capacity;
                    nowSlot = 0;
                }
            }
        }

        private void MousePos(InputAction.CallbackContext ctx) { pointerPos = ctx.ReadValue<Vector2>(); }

        private void StartMove(InputAction.CallbackContext ctx) { moveDir = ctx.ReadValue<Vector2>(); }

        private void StopMove(InputAction.CallbackContext ctx) { moveDir = Vector2.zero; }

        private void Pickup(InputAction.CallbackContext ctx)
        {
            Inventory inv = ((EntityLiving) player).Inventory;
            var it = inv.CheckPickupRadius();
            if (!it.Any())
            {
                inv.DropUsingItem();
            }
            else
            {
                inv.PickupItems(it);
            }
        }

        private void SelectSlot(InputAction.CallbackContext ctx)
        {
            nowSlot = nowSlot >= maxSlot - 1 ? 0 : nowSlot + 1;
            Inventory inv = ((EntityLiving) player).Inventory;
            inv.SelectUsingItem(nowSlot);
        }

        private void StartUseItem(InputAction.CallbackContext ctx) { isUseItem = true; }

        private void StopUseItem(InputAction.CallbackContext ctx) { isUseItem = false; }

        private void RotateInv(InputAction.CallbackContext ctx)
        {
            var mousePos = ctx.ReadValue<Vector2>();
            Vector2 pos = GameManager.Instance.MainCamera.ScreenToWorldPoint(mousePos);
            Inventory inv = ((EntityLiving) player).Inventory;
            inv.Rotate(pos);
        }

        private void RotateBody(InputAction.CallbackContext ctx)
        {
            var mousePos = ctx.ReadValue<Vector2>();
            Vector2 mwPos = GameManager.Instance.MainCamera.ScreenToWorldPoint(mousePos);
            var trans = player.transform;
            Vector2 pPos = trans.position;
            cc2d.Face = mwPos.x > pPos.x ? FaceDirection.Right : FaceDirection.Left;
        }
    }
}