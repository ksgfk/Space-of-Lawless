using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace KSGFK
{
    /// <summary>
    /// 数学扩展
    /// </summary>
    public static class MathExt
    {
        /// <summary>
        /// 单位弧度对应的角度
        /// </summary>
        public const float UnitRadian2Degree = 180 / math.PI;

        /// <summary>
        /// 单位角度对应的弧度
        /// </summary>
        public const float UnitDegree2Radian = math.PI / 180;

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
            var num = math.sqrt(math.lengthsq(from) * (double) math.lengthsq(to));
            return num < 1.00000000362749E-15
                ? 0.0f
                : (float) math.acos(math.clamp(math.dot(@from, to) / num, -1f, 1f)) * UnitRadian2Degree;
        }

        /// <summary>
        /// Mathematics库版本的FromToRotation
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion FromToRotation(float3 from, float3 to)
        {
            var axis = math.cross(from, to);
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
            return quaternion.AxisAngle(math.normalizesafe(nonNormalAxis), degree * UnitDegree2Radian);
        }

        public static float3 QuaternionMulVec3(quaternion q, float3 point)
        {
            var rotation = q.value;
            float num1 = rotation.x * 2f;
            float num2 = rotation.y * 2f;
            float num3 = rotation.z * 2f;
            float num4 = rotation.x * num1;
            float num5 = rotation.y * num2;
            float num6 = rotation.z * num3;
            float num7 = rotation.x * num2;
            float num8 = rotation.x * num3;
            float num9 = rotation.y * num3;
            float num10 = rotation.w * num1;
            float num11 = rotation.w * num2;
            float num12 = rotation.w * num3;
            float3 vector3;
            vector3.x = (float) ((1.0 - ((double) num5 + (double) num6)) * (double) point.x +
                                 ((double) num7 - (double) num12) * (double) point.y +
                                 ((double) num8 + (double) num11) * (double) point.z);
            vector3.y = (float) (((double) num7 + (double) num12) * (double) point.x +
                                 (1.0 - ((double) num4 + (double) num6)) * (double) point.y +
                                 ((double) num9 - (double) num10) * (double) point.z);
            vector3.z = (float) (((double) num8 - (double) num11) * (double) point.x +
                                 ((double) num9 + (double) num10) * (double) point.y +
                                 (1.0 - ((double) num4 + (double) num5)) * (double) point.z);
            return vector3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 TransformDirection(quaternion rotation, float3 dir)
        {
            return QuaternionMulVec3(rotation, dir);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 TransformDirection(Quaternion rotation, Vector3 dir)
        {
            return rotation * dir;
        }
    }
}