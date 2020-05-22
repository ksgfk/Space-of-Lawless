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

        public static int GetLastIndex<T>(this IList<T> list) { return list.Count - 1; }

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

        public static void OnInputCallbackJobMove(this IJobCallback<MoveData> job, InputAction.CallbackContext ctx)
        {
            var j = job.Job;
            ref var movData = ref j[job.DataId];
            movData.Direction = ctx.ReadValue<Vector2>();
        }

        public static void OnInputCallbackJobRotate(this IJobCallback<RotateData> job, InputAction.CallbackContext ctx)
        {
            var j = job.Job;
            ref var rotData = ref j[job.DataId];
            rotData.Delta = ctx.ReadValue<Vector2>();
        }

        public static void OnInputCallbackShipEngineRotate(
            this ShipModuleEngine engine,
            InputAction.CallbackContext ctx)
        {
            var r = ctx.ReadValue<Vector2>();
            r = GameManager.MainCamera.ScreenToWorldPoint(r);
            var pos = (Vector2) engine.BaseShip.transform.position;
            r -= pos;
            var job = (IJobCallback<RotateData>) engine;
            ref var rotData = ref job.Job[job.DataId];
            rotData.Delta = r;
        }
    }
}