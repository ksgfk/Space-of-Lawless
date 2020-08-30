using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVec3(this Vector2 vec2, float z) { return new Vector3(vec2.x, vec2.y, z); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this float f) { return MathExt.IsEqual(f, 0f); }

        public static GameObject Instantiate(this GameObject prefab) { return UnityEngine.Object.Instantiate(prefab); }

        public static GameObject Instantiate(this GameObject prefab, Transform parent)
        {
            return UnityEngine.Object.Instantiate(prefab, parent);
        }

        public static T SafeIndexer<T>(this IList<T> list, int index)
        {
            return index < 0 || index >= list.Count ? default : list[index];
        }

        public static bool TryIndex<T>(this IList<T> list, int index, out T value)
        {
            var isInRange = index >= 0 && index < list.Count;
            value = isInRange ? list[index] : default;
            return isInRange;
        }

        public static Task ToTask(this IAsyncHandleWrapper wrapper)
        {
            return Task.Run(() =>
            {
                while (!wrapper.IsDone)
                {
                    Thread.Sleep(1);
                }

                wrapper.OnComplete();
            });
        }

        // ReSharper disable once InconsistentNaming
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 XY0(this float2 float2) { return new Vector3(float2.x, float2.y, 0); }

        // ReSharper disable once InconsistentNaming
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 xy0(this float2 float2) { return new float3(float2.x, float2.y, 0); }

        public static void RotateMirror(this Transform trans, bool xDir, bool yDir)
        {
            var scale = trans.localScale;
            scale.x = xDir ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            scale.y = yDir ? Mathf.Abs(scale.y) : -Mathf.Abs(scale.y);
            trans.localScale = scale;
        }
    }
}