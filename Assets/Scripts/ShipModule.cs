using System;
using UnityEngine;

namespace KSGFK
{
    public abstract class ShipModule : MonoBehaviour
    {
        [SerializeField] private int moduleId = -1;
        [SerializeField] private EntityShip baseShip = null;

        public int ModuleId
        {
            get => moduleId;
            set
            {
                if (moduleId != -1)
                {
                    throw new InvalidOperationException($"已经初始化过的飞船组件id{moduleId}");
                }

                moduleId = value;
            }
        }

        public EntityShip BaseShip
        {
            get => baseShip;
            set
            {
                if (baseShip)
                {
                    throw new InvalidOperationException($"已经初始化过的飞船组件id{moduleId}");
                }

                baseShip = value;
            }
        }
    }
}