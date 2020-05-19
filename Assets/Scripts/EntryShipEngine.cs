using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public class EntryShipEngine : EntryShipModule
    {
        [SerializeField] private string addr = null;
        [SerializeField] private int pic_size;
        [SerializeField] private float max_mov_speed;
        [SerializeField] private float max_rot_speed;
        [SerializeField] private Sprite asset;

        public string Addr => addr;
        public int PicSize { get => pic_size; set => pic_size = value; }
        public float MaxMoveSpeed { get => max_mov_speed; set => max_mov_speed = value; }
        public float MaxRotateSpeed { get => max_rot_speed; set => max_rot_speed = value; }
        public Sprite Asset { get => asset; set => asset = Helper.SingleAssign(value, asset && value); }

        protected override ShipModule InstantiateBehavior()
        {
            var go = new GameObject($"{RegisterName}:{Id}");
            if (asset)
            {
                var spriteRenderer = go.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = asset;
            }

            var engine = go.AddComponent<ShipModuleEngine>();
            engine.Init(MaxMoveSpeed, MaxRotateSpeed);
            return engine;
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
            string reason = null;
            var result = string.IsNullOrEmpty(Addr) || Helper.CheckResource(Asset, Addr, out reason);
            info = reason;
            return result;
        }
    }
}