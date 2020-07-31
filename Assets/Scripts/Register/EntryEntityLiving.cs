using System;
using UnityEngine;

namespace KSGFK
{
    /// <summary>
    /// 拥有血量的实体注册项
    /// </summary>
    [Serializable]
    public class EntryEntityLiving : EntryEntity
    {
        [ReflectionInject] private string addr = null;
        [ReflectionInject] private ulong max_health = ulong.MinValue;
        private GameObject _prefab;

        public GameObject Prefab => _prefab;

        public override void PerProcess()
        {
            GameManager.Instance.Load.Request<GameObject>(addr, handle => _prefab = Helper.GetAsyncOpResult(handle));
        }

        public override bool Check(out string info)
        {
            var res = Helper.CheckResource(_prefab, addr, out var resInfo);
            var com = Helper.CheckComponent<EntityLiving>(_prefab, out var comInfo);
            info = $"[{resInfo}|{comInfo}]";
            return res && com;
        }

        protected override Entity SpawnEntity()
        {
            var go = UnityEngine.Object.Instantiate(_prefab);
            var entity = go.GetComponent<EntityLiving>();
            entity.MaxHealth = max_health;
            return entity;
        }
    }
}