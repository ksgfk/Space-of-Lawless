using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KSGFK
{
    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(this IList<T> list, int l, int r)
        {
            var t = list[l];
            list[l] = list[r];
            list[r] = t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLastIndex<T>(this IList<T> list) { return list.Count <= 0 ? -1 : list.Count - 1; }

        public static void RemoveAtSwapBack<T>(this IList<T> list, int index)
        {
            var size = list.Count;
            if (size <= 0)
            {
                return;
            }

            var lastIndex = size - 1;
            if (lastIndex == index)
            {
                list.RemoveAt(lastIndex);
                return;
            }

            var lastElement = list[lastIndex];
            list[index] = lastElement;
            list.RemoveAt(lastIndex);
        }

        public static IEnumerable<FieldInfo> GetAllInheritedFields(this Type type, BindingFlags flags)
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
            job.MoveDirection(ctx.ReadValue<Vector2>());
        }

        public static void OnInputCallbackShipEngineRotate(
            this ShipModuleEngine engine,
            InputAction.CallbackContext ctx)
        {
            var r = ctx.ReadValue<Vector2>();
            r = GameManager.MainCamera.ScreenToWorldPoint(r);
            var pos = (Vector2) engine.BaseShip.transform.position;
            r -= pos;
            engine.RotateDelta(r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVec3(this Vector2 vec2, float z) { return new Vector3(vec2.x, vec2.y, z); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this float f) { return math.abs(f) <= 0.0000001f; }
    }
}