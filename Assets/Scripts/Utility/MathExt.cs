using System;
using System.Runtime.CompilerServices;
using MPipeline;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace KSGFK
{
    /// <summary>
    /// 数学扩展
    /// </summary>
    public static class MathExt
    {
        /// <summary>
        /// 分量z置零
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToZeroZ(this Vector3 src)
        {
            src.z = 0;
            return src;
        }

        /// <summary>
        /// 向量夹角
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(float3 from, float3 to)
        {
            var denominator = sqrt(lengthsq(from) * lengthsq(to));
            if (denominator < FLT_MIN_NORMAL)
            {
                return 0F;
            }

            var d = clamp(dot(from, to) / denominator, -1F, 1F);
            return degrees(acos(d));
        }

        /// <summary>
        /// Mathematics库版本的FromToRotation
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion FromToRotation(float3 from, float3 to)
        {
            var axis = cross(from, to);
            var angle = Angle(from, to);
            return NonInitAxisAngle(axis, angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FromToRotation(Vector3 from, Vector3 to)
        {
            var axis = Vector3.Cross(from, to);
            var angle = Vector3.Angle(from, to);
            return Quaternion.AngleAxis(angle, axis.normalized);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion NonInitAxisAngle(float3 nonNormalAxis, float degree)
        {
            return Unity.Mathematics.quaternion.AxisAngle(normalizesafe(nonNormalAxis), radians(degree));
        }

        public static float3 QuaternionMulVec3(quaternion q, float3 point)
        {
            var rotation = q.value;
            var xyz = rotation.xyz * 2f;
            var xxyyzz = rotation.xyz * xyz;
            var xyxzyz = rotation.xxy * xyz.yzz;
            var wxwywz = rotation.w * xyz;
            var m = new float3x3(1F - (xxyyzz.y + xxyyzz.z), //11
                xyxzyz.x - wxwywz.z, //12
                xyxzyz.y + wxwywz.y, //13
                xyxzyz.x + wxwywz.z, //21
                1F - (xxyyzz.x + xxyyzz.z), //22
                xyxzyz.z - wxwywz.x, //23
                xyxzyz.y - wxwywz.y, //31
                xyxzyz.z + wxwywz.x, //32
                1F - (xxyyzz.x + xxyyzz.y)); //33
            return mul(m, point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 TransformDirection(quaternion rotation, float3 dir)
        {
            return QuaternionMulVec3(rotation, dir);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 TransformDirection(Quaternion rotation, Vector3 dir) { return rotation * dir; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 DumpMove(in Vector2 origin, in Vector2 target, float dump, float deltaTime)
        {
            return Vector2.Lerp(origin, target, deltaTime * dump);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 DumpMove(in float2 origin, in float2 target, float dump, float deltaTime)
        {
            return lerp(origin, target, deltaTime * dump);
        }

        /// <summary>
        /// 绕原点旋转向量
        /// </summary>
        /// <param name="raw">需要旋转的向量</param>
        /// <param name="angle">旋转角度</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Rotate(Vector2 raw, float angle)
        {
            sincos(radians(angle), out var sin, out var cos);
            return Mul(Matrix2X2(cos, -sin, sin, cos), raw);
        }

        /// <summary>
        /// 2x2与2x1矩阵乘法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Mul(Vector4 mat2X2, Vector2 vec2)
        {
            return new Vector2(mat2X2.x, mat2X2.z) * vec2.x + new Vector2(mat2X2.y, mat2X2.w) * vec2.y;
        }

        /// <summary>
        /// 构造2x2矩阵
        /// </summary>
        /// <param name="m00">矩阵中1行1列值</param>
        /// <param name="m01">矩阵中1行2列值</param>
        /// <param name="m10">矩阵中2行1列值</param>
        /// <param name="m11">矩阵中2行2列值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Matrix2X2(float m00, float m01, float m10, float m11)
        {
            return new Vector4(m00, m01, m10, m11);
        }

        /// <summary>
        /// 坐标系转换，以y轴为基准
        /// </summary>
        /// <param name="targetCoord">目标坐标系</param>
        /// <param name="vec2">需要转换的向量</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ConvertCoord(Vector2 targetCoord, Vector2 vec2)
        {
            var angle = Vector2.SignedAngle(Vector2.up, targetCoord);
            return Rotate(vec2, angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSmallValZero(ref Vector2 vec2)
        {
            if (vec2.x.IsZero())
            {
                vec2.x = 0f;
            }

            if (vec2.y.IsZero())
            {
                vec2.y = 0f;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEqual(float val, float tar) { return abs(val - tar) <= 0.0000001f; }

        public static float SignedAngle(float3 from, float3 to, float3 axis)
        {
            var unsignedAngle = Angle(from, to);
            var a = cross(from, to) * axis;
            var sign = Mathf.Sign(a.x + a.y + a.z);
            return unsignedAngle * sign;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(ref T l, ref T r)
        {
            var t = l;
            l = r;
            r = t;
        }

        public static unsafe void Quicksort<T>(T* a, int p, int q) where T : unmanaged, IFunction<T, int>
        {
            int i = p;
            int j = q;
            T temp = a[p];

            while (i < j)
            {
                while (a[j].Run(ref temp) >= 0 && j > i) j--;

                if (j > i)
                {
                    a[i] = a[j];
                    i++;
                    while (a[i].Run(ref temp) <= 0 && i < j) i++;
                    if (i < j)
                    {
                        a[j] = a[i];
                        j--;
                    }
                }
            }

            a[i] = temp;
            if (p < (i - 1)) Quicksort(a, p, i - 1);
            if ((j + 1) < q) Quicksort(a, j + 1, q);
        }

        public static unsafe void Quicksort(int* a, int p, int q)
        {
            int i = p;
            int j = q;
            int temp = a[p];

            while (i < j)
            {
                while (a[j] >= temp && j > i) j--;

                if (j > i)
                {
                    a[i] = a[j];
                    i++;
                    while (a[i] <= temp && i < j) i++;
                    if (i < j)
                    {
                        a[j] = a[i];
                        j--;
                    }
                }
            }

            a[i] = temp;
            if (p < (i - 1)) Quicksort(a, p, i - 1);
            if ((j + 1) < q) Quicksort(a, j + 1, q);
        }

        public static unsafe int BinarySearch<T>(T* arr, int start, int end, ref T target)
            where T : unmanaged, IFunction<T, int>
        {
            var left = start;
            var right = end;
            while (true)
            {
                var mid = (right + left) / 2;
                if (arr[mid].Run(ref target) == 0)
                {
                    return mid;
                }

                if (left > right)
                {
                    return -1;
                }

                if (arr[mid].Run(ref target) >= 0)
                {
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }
        }

        public static unsafe int BinarySearch<T>(T* arr, int start, int end, T target)
            where T : unmanaged, IComparable<T>
        {
            var left = start;
            var right = end;
            while (true)
            {
                var mid = (right + left) / 2;
                if (arr[mid].CompareTo(target) == 0)
                {
                    return mid;
                }

                if (left > right)
                {
                    return -1;
                }

                if (arr[mid].CompareTo(target) >= 0)
                {
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }
        }
    }
}