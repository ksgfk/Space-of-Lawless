using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public class EntryEntityLiving : EntryEntity
    {
        [SerializeField] private string addr = null;
        [SerializeField] private ulong max_health = ulong.MinValue;
        [SerializeField] private GameObject prefab = null;

        public override void PerProcess() { GameManager.Load.Request(addr, (GameObject go) => prefab = go); }

        public override bool Check(out string info)
        {
            var res = Helper.CheckResource(prefab, addr, out var resInfo);
            var com = Helper.CheckComponent<EntityLiving>(prefab, out var comInfo);
            info = $"[{resInfo}|{comInfo}]";
            return res && com;
        }

        protected override Entity SpawnEntity()
        {
            var go = UnityEngine.Object.Instantiate(prefab);
            var entity = go.GetComponent<EntityLiving>();
            entity.MaxHealth = max_health;
            return entity;
        }
    }
}