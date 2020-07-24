using System.Runtime.CompilerServices;
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
            var num = sqrt(lengthsq(from) * (double) lengthsq(to));
            return num < 1.00000000362749E-15
                ? 0.0f
                : (float) acos(clamp(dot(from, to) / num, -1f, 1f)) * UnitRadian2Degree;
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
            return Unity.Mathematics.quaternion.AxisAngle(normalizesafe(nonNormalAxis), degree * UnitDegree2Radian);
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
    }
}