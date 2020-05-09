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
            return quaternion.AxisAngle(math.normalize(nonNormalAxis), degree * UnitDegree2Radian);
        }
    }
}