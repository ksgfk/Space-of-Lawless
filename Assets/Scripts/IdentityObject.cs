using System;
using UnityEngine;

namespace KSGFK
{
    public class IdentityObject : MonoBehaviour, IRuntimeIdentity
    {
        [SerializeField] private int _runtimeId = -1;

        public int RuntimeId => _runtimeId;

        public void SetId(int id)
        {
            if (_runtimeId <= -1)
            {
                _runtimeId = id;
            }
            else
            {
                if (id < 0)
                {
                    _runtimeId = -1;
                }
                else
                {
                    throw new InvalidOperationException("不可重复赋值");
                }
            }
        }
    }
}