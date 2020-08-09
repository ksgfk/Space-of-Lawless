using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace KSGFK
{
    [Serializable]
    public readonly struct EntityLivingInfo
    {
        public readonly string Name;
        public readonly string Addr;
        public readonly ulong MaxHealth;

        public EntityLivingInfo(string name, string addr, ulong maxHealth)
        {
            Name = name;
            Addr = addr;
            MaxHealth = maxHealth;
        }
    }

    public class EntryEntityLiving : EntryEntity
    {
        private readonly EntityLivingInfo _info;
        private GameObject _prefab;

        public override string RegisterName => _info.Name;

        public string AssetAddr => _info.Addr;
        public ulong MaxHealth => _info.MaxHealth;
        public GameObject Prefab => _prefab;

        public EntryEntityLiving(EntityLivingInfo info) { _info = info; }

        protected override Entity SpawnEntity()
        {
            var go = UnityEngine.Object.Instantiate(_prefab);
            var entity = go.GetComponent<EntityLiving>();
            entity.SetMaxHealth(MaxHealth);
            return entity;
        }

        public override Task PreProcess()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(AssetAddr);
            handle.Completed += h => _prefab = Helper.GetAsyncOpResult(h);
            return handle.Task;
        }

        public override bool Check(out string reason)
        {
            var res = Helper.CheckResource(_prefab, AssetAddr, out var resInfo);
            var com = Helper.CheckComponent<EntityLiving>(_prefab, out var comInfo);
            reason = $"{resInfo}|{comInfo}";
            return res && com;
        }
    }
}