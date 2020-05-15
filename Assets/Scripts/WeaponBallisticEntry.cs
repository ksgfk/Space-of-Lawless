using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public class WeaponBallisticEntry : ShipModuleEntry
    {
        [SerializeField] private int runtimeId;
        [SerializeField] private string name = null;
        [SerializeField] private string weapon_addr = null;
        [SerializeField] private int weapon_pic_size = 0;
        [SerializeField] private string bullet_addr = null;
        [SerializeField] private int bullet_pic_size = 0;
        [SerializeField] private float speed = 0;
        [SerializeField] private int bullet_pooling = -1;
        [SerializeField] private Sprite weapon_Asset;
        [SerializeField] private Sprite bullet_asset;
        [SerializeField] private int bulletPoolId;
        [SerializeField] private GameManager bulletPrefab;

        public override int Id { get => runtimeId; set => runtimeId = value; }
        public override string RegisterName => name;
        public string WeaponAddr => weapon_addr;
        public int WeaponPicSize => weapon_pic_size;
        public float Speed => speed;
        public int BulletPooling => bullet_pooling;
        public string BulletAddr => bullet_addr;
        public int BulletPicSize => bullet_pic_size;

        public Sprite WeaponAsset
        {
            get => weapon_Asset;
            set
            {
                if (weapon_Asset && value)
                {
                    throw new InvalidOperationException("不可重复对需加载资源赋值");
                }

                weapon_Asset = value;
            }
        }

        public Sprite BulletAsset
        {
            get => bullet_asset;
            set
            {
                if (bullet_asset && value)
                {
                    throw new InvalidOperationException("不可重复对需加载资源赋值");
                }

                bullet_asset = value;
            }
        }

        public override void PerProcess()
        {
            if (!string.IsNullOrEmpty(WeaponAddr))
            {
                GameManager.Load.Request<Sprite>(WeaponAddr, sprite => WeaponAsset = sprite);
            }

            GameManager.Load.Request<Sprite>(BulletAddr, sprite => WeaponAsset = sprite);
        }

        public override void Process()
        {
            if (GameManager.Pool.IsAllocated(BulletAddr))
            {
                return;
            }

            var bgo = new GameObject(RegisterName);
            var bgoSprite = bgo.AddComponent<SpriteRenderer>();
            bgoSprite.sprite = BulletAsset;
            GameManager.Pool.Allocate(bgo, BulletAddr, BulletPooling, newId => bulletPoolId = newId);
        }

        public override bool Check(out string info)
        {
            var result = true;
            var reason = string.Empty;
            if (!BulletAsset)
            {
                reason += $"未成功加载资源{BulletAddr},忽略";
                result = false;
            }

            if (!string.IsNullOrEmpty(WeaponAddr) && !WeaponAsset)
            {
                reason += $"|未成功加载资源{BulletAddr},忽略";
                result = false;
            }

            info = reason;
            return result;
        }

        public override IShipModule Instantiate() { throw new NotImplementedException(); }

        public override void Destroy(IShipModule instance) { throw new NotImplementedException(); }
    }
}