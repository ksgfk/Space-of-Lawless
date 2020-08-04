using System;
using UnityEngine;

namespace KSGFK
{
    public class IdentityObject : MonoBehaviour
    {
        [SerializeField] private int runtimeId = -1;

        public int RuntimeId
        {
            get => runtimeId;
            set
            {
                if (runtimeId <= -1)
                {
                    runtimeId = value;
                }
                else
                {
                    if (value < 0)
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
}