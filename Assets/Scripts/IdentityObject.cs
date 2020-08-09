using System;
using UnityEngine;

namespace KSGFK
{
    public class IdentityObject : MonoBehaviour, IRuntimeIdentity
    {
        [SerializeField] private int runtimeId = -1;

        public int RuntimeId => runtimeId;

        public void SetupId(int id)
        {
            if (runtimeId <= -1)
            {
                runtimeId = id;
            }
            else
            {
                if (id < 0)
                {
                    runtimeId = -1;
                }
                else
                {
                    throw new InvalidOperationException("不可重复赋值");
                }
            }
        }
    }
}