using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public class ShipEngineEntry : ShipModuleEntry
    {
        [SerializeField] private int runtimeId;
        [SerializeField] private string name = null;
        [SerializeField] private string addr = null;
        [SerializeField] private int pic_size;
        [SerializeField] private float max_mov_speed;
        [SerializeField] private float max_rot_speed;
        [SerializeField] private Sprite asset;

        public string Addr => addr;
        public int PicSize { get => pic_size; set => pic_size = value; }
        public float MaxMoveSpeed { get => max_mov_speed; set => max_mov_speed = value; }
        public float MaxRotateSpeed { get => max_rot_speed; set => max_rot_speed = value; }

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

        public override int Id { get => runtimeId; set => runtimeId = value; }

        public override string RegisterName => name;

        public override IShipModule Instantiate()
        {
            var go = new GameObject($"{name}:{runtimeId}");
            if (asset)
            {
                var spriteRenderer = go.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = asset;
            }

            var engine = go.AddComponent<ShipEngine>();
            engine.maxMoveSpeed = MaxMoveSpeed;
            engine.maxRotateSpeed = MaxRotateSpeed;
            return engine;
        }

        public override void Destroy(IShipModule instance) { UnityEngine.Object.Destroy(instance.BaseGameObject); }

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
            bool result;
            string mInfo = null;
            if (!string.IsNullOrEmpty(Addr))
            {
                if (Asset)
                {
                    result = true;
                }
                else
                {
                    mInfo = $"未成功加载资源{Addr},忽略";
                    result = false;
                }
            }
            else
            {
                result = true;
            }


            info = mInfo;
            return result;
        }
    }
}