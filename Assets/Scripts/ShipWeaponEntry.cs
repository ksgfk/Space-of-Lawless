using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public class ShipWeaponEntry : ShipModuleEntry
    {
        [SerializeField] private int runtimeId;
        [SerializeField] private string name = null;
        [SerializeField] private string addr = null;
        [SerializeField] private int pic_size = 0;
        [SerializeField] private Sprite asset;

        public override int Id { get => runtimeId; set => runtimeId = value; }
        public override string RegisterName => name;
        public string Addr => addr;
        public int PicSize => pic_size;

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

        public override void PerProcess()
        {
            if (!string.IsNullOrEmpty(Addr))
            {
                GameManager.Load.Request<Sprite>(Addr, sprite => Asset = sprite);
            }
        }

        public override void Process() { }

        public override bool Check(out string info)
        {
            var result = true;
            var reason = string.Empty;
            if (!string.IsNullOrEmpty(Addr))
            {
                if (!Asset)
                {
                    reason += $"未成功加载资源{Addr},忽略";
                    result = false;
                }
            }

            info = reason;
            return result;
        }

        public override IShipModule Instantiate() { throw new InvalidOperationException(); }
    }
}