using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public class EntryEntityBullet : EntryEntity
    {
        [ReflectionInject] private string addr = null;
        [ReflectionInject] private bool is_pool = false;
        private GameObject _prefab;
        private int _poolId = -1;

        public string Addr => addr;
        public bool IsToPool => is_pool;
        public int PoolId => _poolId;
        public GameObject Prefab => _prefab;

        protected override Entity SpawnEntity()
        {
            EntityBullet result;
            World world = GameManager.Instance.World;
            var poolObjId = -1;
            if (IsToPool)
            {
                if (PoolId <= -1)
                {
                    _poolId = world.Pool.Allocate(Prefab, RegisterName, 1);
                    world.Unload += _ => _poolId = -1;
                }

                var pool = world.Pool[PoolId];
                poolObjId = pool.Get();
                var go = pool.Container[poolObjId];
                result = go.GetComponent<EntityBullet>();
            }
            else
            {
                result = UnityEngine.Object.Instantiate(Prefab).GetComponent<EntityBullet>();
            }

            result.PoolObjectId = poolObjId;
            return result;
        }

        protected override void DestroyEntity(Entity instance)
        {
            var bullet = (EntityBullet) instance;
            World world = GameManager.Instance.World;
            if (IsToPool)
            {
                if (PoolId <= -1)
                {
                    throw new ArgumentException("非法的池Id，可能有bug");
                }

                if (bullet.PoolObjectId <= -1)
                {
                    throw new InvalidOperationException($"{RegisterName}:{PoolId}不能回收无id的子弹{instance}");
                }

                world.Pool.Return(PoolId, bullet.PoolObjectId);
                bullet.PoolObjectId = -1;
                bullet.RuntimeId = -1;
                bullet.Node = null;
            }
            else
            {
                base.DestroyEntity(instance);
            }
        }

        public override void PerProcess()
        {
            GameManager.Instance.Load.Request<GameObject>(Addr, handle => _prefab = Helper.GetAsyncOpResult(handle));
        }

        public override bool Check(out string info)
        {
            var result = Helper.CheckResource(Prefab, Addr, out var resReason);
            var comp = Helper.CheckComponent<EntityBullet>(Prefab, out var compReason);
            info = $"[{resReason}|{compReason}]";
            return result && comp;
        }
    }
}