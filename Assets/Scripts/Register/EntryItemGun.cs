using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace KSGFK
{
    [Serializable]
    public readonly struct ItemGunInfo
    {
        public readonly string Name;
        public readonly string Addr;
        public readonly float Damage;
        public readonly float RateOfFire;
        public readonly int MagazineCapacity;

        public ItemGunInfo(string name, string addr, float damage, float rateOfFire, int magazineCapacity)
        {
            Name = name;
            Addr = addr;
            Damage = damage;
            RateOfFire = rateOfFire;
            MagazineCapacity = magazineCapacity;
        }

        public static implicit operator GunInfo(ItemGunInfo gun)
        {
            return new GunInfo
            {
                Damage = gun.Damage,
                MagazineCapacity = gun.MagazineCapacity,
                RateOfFire = gun.RateOfFire
            };
        }
    }

    public class EntryItemGun : EntryItem
    {
        private readonly ItemGunInfo _info;
        private GameObject _prefab;

        public override string RegisterName => _info.Name;
        public string AssetAddr => _info.Addr;
        public ItemGunInfo GunInfo => _info;
        public GameObject Prefab => _prefab;

        public EntryItemGun(ItemGunInfo info) { _info = info; }

        public override Task PreProcess()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(AssetAddr);
            handle.Completed += h => _prefab = Helper.GetAsyncOpResult(h);
            return handle.Task;
        }

        public override bool Check(out string reason)
        {
            var res = Helper.CheckResource(_prefab, AssetAddr, out var resInfo);
            var com = Helper.CheckComponent<ItemGun>(_prefab, out var comInfo);
            reason = $"{resInfo}|{comInfo}";
            return res && com;
        }

        protected override Item CreateItem()
        {
            var item = UnityEngine.Object.Instantiate(Prefab).GetComponent<ItemGun>();
            item.SetGunInfo(_info);
            return item;
        }
    }
}