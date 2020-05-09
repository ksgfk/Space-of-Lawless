using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public class ShipFrameData
    {
        public string id;
        public string addr;
        public float pic_size;
        public ulong max_health;
        public int max_module;
        [SerializeField] private Sprite asset;

        public Sprite Asset
        {
            get => asset;
            set
            {
                if (asset && value)
                {
                    throw new InvalidOperationException("不可重复对需加载资源赋值");
                }

                asset = value;
            }
        }

        public string Id => id;
    }
}