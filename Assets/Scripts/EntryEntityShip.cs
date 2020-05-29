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
        [SerializeField] private GameObject prefab = null;

        public string Addr => addr;
        public float PicSize => pic_size;
        public ulong MaxHealth => max_health;
        public GameObject Prefab { get => prefab; set => prefab = Helper.SingleAssign(value, prefab && value); }

        protected override Entity SpawnEntity()
        {
            var go = UnityEngine.Object.Instantiate(Prefab);
            var ship = go.GetComponent<EntityShip>();
            ship.Health = MaxHealth;
            return ship;
        }

        protected override void DestroyEntity(Entity instance) { UnityEngine.Object.Destroy(instance.gameObject); }

        public override void PerProcess() { GameManager.Load.Request<GameObject>(Addr, go => Prefab = go); }

        public override void Process() { }

        public override bool Check(out string info)
        {
            bool result;
            string reason;
            if (Prefab.TryGetComponent<EntityShip>(out _))
            {
                result = Helper.CheckResource(Prefab, Addr, out reason);
            }
            else
            {
                result = false;
                reason = $"子弹{RegisterName}不存在{typeof(EntityBullet)}组件";
            }

            info = reason;
            return result;
        }
    }
}