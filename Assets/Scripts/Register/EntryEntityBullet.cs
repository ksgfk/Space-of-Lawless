using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public class EntryEntityBullet : EntryEntity
    {
        [SerializeField] private string addr = null;
        [SerializeField] private int pool_count = -1;
        [SerializeField] private GameObject asset;
        [SerializeField] private int poolId;

        public string Addr => addr;
        public int PoolCount => pool_count;
        public int PoolId => poolId;

        public GameObject Asset { get => asset; set => asset = Helper.SingleAssign(value, asset && value); }

        protected override Entity SpawnEntity()
        {
            EntityBullet result;
            if (PoolCount < 0)
            {
                result = UnityEngine.Object.Instantiate(Asset).GetComponent<EntityBullet>();
                result.PoolObjectId = -1;
            }
            else
            {
                var myPool = GameManager.Pool.Pools[PoolId];
                var ptr = myPool.Get();
                var go = myPool.Container[ptr];
                var b = go.GetComponent<EntityBullet>();
                b.PoolObjectId = ptr;
                result = b;
            }

            return result;
        }

        protected sealed override void DestroyEntity(Entity instance)
        {
            var bullet = (EntityBullet) instance;
            if (PoolCount < 0)
            {
                UnityEngine.Object.Destroy(instance.gameObject);
            }
            else
            {
                if (bullet.PoolObjectId < 0)
                {
                    throw new InvalidOperationException($"{RegisterName}:{PoolId}不能回收无id的子弹{instance}");
                }

                GameManager.Pool.Return(PoolId, bullet.PoolObjectId);
                bullet.PoolObjectId = -1;
                bullet.RuntimeId = -1;
                bullet.Node = null;
            }
        }

        public override void PerProcess() { GameManager.Load.Request<GameObject>(Addr, ass => Asset = ass); }

        public override void Process()
        {
            if (!Asset.TryGetComponent<EntityBullet>(out _))
            {
                Debug.Log($"子弹{RegisterName}不存在{typeof(EntityBullet)}组件");
                return;
            }

            poolId = GameManager.Pool.Allocate(Asset, RegisterName, PoolCount, newId => poolId = newId);
        }

        public override bool Check(out string info)
        {
            var result = Helper.CheckResource(Asset, Addr, out var reason);
            if (!GameManager.Pool.IsAllocated(RegisterName))
            {
                reason += $" 未成功分配资源池{RegisterName},忽略";
                result = false;
            }

            info = reason;
            return result;
        }
    }
}