using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KSGFK
{
    public static class Extensions
    {
        public static void Swap<T>(this IList<T> list, int l, int r)
        {
            var t = list[l];
            list[l] = list[r];
            list[r] = t;
        }

        public static int GetLastIndex<T>(this IList<T> list) { return list.Count <= 0 ? 0 : list.Count - 1; }

        public static T GetLastElement<T>(this IList<T> list)
        {
            return list.Count <= 0 ? default : list[list.Count - 1];
        }

        public static void RemoveSwapLast<T>(this IList<T> list, int index)
        {
            var last = list.GetLastIndex();
            list.Swap(index, last);
            list.RemoveAt(last);
        }

        public static IEnumerable<FieldInfo> GetAllFields(this Type type, BindingFlags flags)
        {
            var loopType = type;
            var list = new List<FieldInfo>();
            while (loopType != null && loopType != typeof(object))
            {
                var fields = loopType.GetFields(flags);
                list.AddRange(fields);
                loopType = loopType.BaseType;
            }

            return list;
        }

        public static void OnInputCallbackFireStart(this ShipModuleWeapon weapon, InputAction.CallbackContext ctx)
        {
            weapon.CanFire = true;
        }

        public static void OnInputCallbackFireCancel(this ShipModuleWeapon weapon, InputAction.CallbackContext ctx)
        {
            weapon.CanFire = false;
        }

        public static void OnInputCallbackJobMove(this ShipModuleEngine job, InputAction.CallbackContext ctx)
        {
            job.SetMoveDirection(ctx.ReadValue<Vector2>());
        }

        public static void OnInputCallbackShipEngineRotate(
            this ShipModuleEngine engine,
            InputAction.CallbackContext ctx)
        {
            var r = ctx.ReadValue<Vector2>();
            r = GameManager.MainCamera.ScreenToWorldPoint(r);
            var pos = (Vector2) engine.BaseShip.transform.position;
            r -= pos;
            engine.SetRotateDelta(r);
        }
    }
}