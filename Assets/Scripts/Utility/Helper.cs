using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace KSGFK
{
    public static class Helper
    {
        public static bool CheckResource(UnityEngine.Object asset, string addr, out string info)
        {
            bool result;
            var mInfo = string.Empty;
            if (asset)
            {
                result = true;
            }
            else
            {
                mInfo = $"未成功加载资源{addr},忽略";
                result = false;
            }

            info = mInfo;
            return result;
        }

        public static bool CheckComponent<T>(GameObject go, out string info) where T : Component
        {
            bool result;
            var mInfo = string.Empty;
            if (go.TryGetComponent<T>(out _))
            {
                result = true;
            }
            else
            {
                result = false;
                mInfo = $"GO{go.name}不存在{typeof(T).FullName}组件";
            }

            info = mInfo;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SingleAssign<T>(T value, bool canAssign)
        {
            if (canAssign)
            {
                throw new InvalidOperationException("不可重复赋值");
            }

            return value;
        }

        [Conditional("DEBUG_RAYS")]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color)
        {
            UnityEngine.Debug.DrawRay(start, dir, color);
        }

        public static T[] NewArray<T>(int size, Func<T> defaultVal)
        {
            var arr = new T[size];
            for (var i = 0; i < size; i++)
            {
                arr[i] = defaultVal();
            }

            return arr;
        }
    }
}