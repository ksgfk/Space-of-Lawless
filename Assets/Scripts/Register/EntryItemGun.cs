using System;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace KSGFK
{
    [Serializable]
    public class ItemGunInfo : ItemInfo
    {
        public float Damage { get; set; }
        public float RateOfFire { get; set; }
        public float Reload { get; set; }
        public int MagazineCapacity { get; set; }
        public string EntityBulletName { get; set; }
        public AmmoType UsedAmmo { get; set; }

        public static implicit operator GunInfo(ItemGunInfo gun)
        {
            return new GunInfo
            {
                Damage = gun.Damage,
                MagazineCapacity = gun.MagazineCapacity,
                RateOfFire = gun.RateOfFire,
                EntityBulletName = gun.EntityBulletName,
                Reload = gun.Reload,
                UsedAmmo = gun.UsedAmmo
            };
        }
    }

    public class ItemGunInfoMap : ClassMap<ItemGunInfo>
    {
        public ItemGunInfoMap()
        {
            Map(info => info.Name);
            Map(info => info.Addr);
            Map(info => info.MaxStack);
            Map(info => info.CollideRadius);
            Map(info => info.Damage);
            Map(info => info.RateOfFire);
            Map(info => info.Reload);
            Map(info => info.MagazineCapacity);
            Map(info => info.EntityBulletName);
            Map(info => info.UsedAmmo).TypeConverter<AmmoTypeConverter>();
        }
    }

    public sealed class EntryItemGun : EntryItem
    {
        private readonly ItemGunInfo _info;

        public ItemGunInfo GunInfo => _info;

        public EntryItemGun(ItemGunInfo info) : base(info) { _info = info; }

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