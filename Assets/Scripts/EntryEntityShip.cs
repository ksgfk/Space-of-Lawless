using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public class EntryEntityShip : EntryEntity
    {
        [SerializeField] private string addr = null;
        [SerializeField] private float pic_size = float.NaN;
        [SerializeField] private ulong max_health = ulong.MaxValue;
        [SerializeField] private Sprite asset = null;

        public string Addr => addr;
        public float PicSize => pic_size;
        public ulong MaxHealth => max_health;
        public Sprite Asset { get => asset; set => asset = Helper.SingleAssign(value, asset && value); }

        protected override Entity InstantiateBehavior()
        {
            var go = new GameObject($"{RegisterName}:{Id}");
            var scale = 120f * pic_size / Asset.texture.width;
            go.transform.localScale = new Vector3(scale, scale, scale);
            var spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Asset;
            var ship = go.AddComponent<EntityShip>();
            ship.Health = MaxHealth;
            return ship;
        }

        public override void PerProcess() { GameManager.Load.Request<Sprite>(Addr, sprite => Asset = sprite); }

        public override void Process() { }

        public override bool Check(out string info) { return Helper.CheckResource(Asset, Addr, out info); }
    }
}