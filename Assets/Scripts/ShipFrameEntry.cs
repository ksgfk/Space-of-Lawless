using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public class ShipFrameEntry : EntityEntry<EntityShip>
    {
        [SerializeField] private int id = int.MinValue;
        [SerializeField] private string name = null;
        [SerializeField] private string addr = null;
        [SerializeField] private float pic_size = float.NaN;
        [SerializeField] private ulong max_health = ulong.MaxValue;
        [SerializeField] private int max_module = int.MinValue;
        [SerializeField] private Sprite asset = null;

        public override int Id { get => id; set => id = value; }
        public override string RegisterName => name;
        public string Addr => addr;
        public float PicSize => pic_size;
        public ulong MaxHealth => max_health;
        public int MaxModule => max_module;

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

        public override EntityShip Instantiate() { throw new NotImplementedException(); }
    }

    public class ShipFrameProcessor : IProcessor
    {
        public void ProProcess(object target)
        {
            if (target is ShipFrameEntry frame)
            {
                GameManager.Load.Request<Sprite>(frame.Addr, sprite => frame.Asset = sprite);
            }
            else
            {
                Debug.LogWarningFormat("{0}不是{1},忽略", target, typeof(ShipFrameEntry));
            }
        }

        public void Process(object target) { }

        public bool Check(object target, out string info)
        {
            bool result;
            string mInfo = null;
            if (target is ShipFrameEntry frame)
            {
                if (frame.Asset)
                {
                    result = true;
                }
                else
                {
                    mInfo = $"未成功加载资源{frame.Addr},忽略";
                    result = false;
                }
            }
            else
            {
                mInfo = $"{target}不是{typeof(ShipFrameEntry)},忽略";
                result = false;
            }

            info = mInfo;
            return result;
        }
    }
}