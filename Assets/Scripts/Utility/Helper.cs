using System;
using System.Runtime.CompilerServices;

namespace KSGFK
{
    public static class Helper
    {
        public static bool CheckResource(UnityEngine.Object asset, string addr, out string info)
        {
            bool result;
            string mInfo = null;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SingleAssign<T>(T value, bool canAssign)
        {
            if (canAssign)
            {
                throw new InvalidOperationException("不可重复赋值");
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T GetDataFromJob<T>(IJobCallback<JobTemplate<T>> jobUser) where T : unmanaged
        {
            return ref jobUser.Job[jobUser.DataId];
        }
    }
}