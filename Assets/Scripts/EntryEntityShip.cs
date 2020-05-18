using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public class EntryEntityShip : EntityRegisterEntry
    {
        [SerializeField] private int id = int.MinValue;
        [SerializeField] private string name = null;
        [SerializeField] private string addr = null;
        [SerializeField] private float pic_size = float.NaN;
        [SerializeField] private ulong max_health = ulong.MaxValue;
        [SerializeField] private Sprite asset = null;

        public override int Id { get => id; set => id = value; }
        public override string RegisterName => name;
        public string Addr => addr;
        public float PicSize => pic_size;
        public ulong MaxHealth => max_health;

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

        public override Entity Instantiate()
        {
            var go = new GameObject($"{name}:{id}");
            var scale = 120f * pic_size / Asset.texture.width;
            go.transform.localScale = new Vector3(scale, scale, scale);
            var spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Asset;
            var ship = go.AddComponent<EntityShip>();
            ship.RuntimeId = Id;
            ship.Health = MaxHealth;
            return ship;
        }

        public override void PerProcess() { GameManager.Load.Request<Sprite>(Addr, sprite => Asset = sprite); }

        public override void Process() { }

        public override bool Check(out string info)
        {
            bool result;
            string mInfo = null;
            if (Asset)
            {
                result = true;
            }
            else
            {
                mInfo = $"未成功加载资源{Addr},忽略";
                result = false;
            }
            
            info = mInfo;
            return result;
        }
    }
}