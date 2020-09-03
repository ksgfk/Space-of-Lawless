using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace KSGFK
{
    [Serializable]
    public class ItemGunInfo
    {
        public readonly string Name;
        public readonly string Addr;
        public readonly float CollideRadius;
        public readonly int MaxStack;
        public readonly float Damage;
        public readonly float RateOfFire;
        public readonly int MagazineCapacity;
        public readonly string BulletName;

        public ItemGunInfo(
            string name,
            string addr,
            float collideRadius,
            int maxStack,
            float damage,
            float rateOfFire,
            int magazineCapacity,
            string bulletName)
        {
            Name = name;
            Addr = addr;
            Damage = damage;
            RateOfFire = rateOfFire;
            MagazineCapacity = magazineCapacity;
            BulletName = bulletName;
            CollideRadius = collideRadius;
            MaxStack = maxStack;
        }

        [Obsolete("用于反射", true)]
        public ItemGunInfo() { }

        public static implicit operator GunInfo(ItemGunInfo gun)
        {
            return new GunInfo
            {
                Damage = gun.Damage,
                MagazineCapacity = gun.MagazineCapacity,
                RateOfFire = gun.RateOfFire,
                BulletName = gun.BulletName
            };
        }
    }

    public sealed class EntryItemGun : EntryItem
    {
        private readonly ItemGunInfo _info;

        public ItemGunInfo GunInfo => _info;

        public EntryItemGun(ItemGunInfo info) : base(new ItemInfo(info.Name,
            info.Addr,
            info.MaxStack,
            info.CollideRadius))
        {
            _info = info;
        }

        public override Task PreProcess()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(BaseInfo.Addr);
            handle.Completed += h => MPrefab = Helper.GetAsyncOpResult(h);
            return handle.Task;
        }

        public override bool Check(out string reason)
        {
            var res = Helper.CheckResource(MPrefab, BaseInfo.Addr, out var resInfo);
            var com = Helper.CheckComponent<ItemGun>(Prefab, out var comInfo);
            reason = $"{resInfo}|{comInfo}";
            return res && com;
        }

        protected override Item CreateItem()
        {
            var item = UnityEngine.Object.Instantiate(Prefab).GetComponent<ItemGun>();
            item.SetGunInfo(GunInfo);
            return item;
        }
    }
}