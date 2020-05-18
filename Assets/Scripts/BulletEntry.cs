using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public class BulletEntry : EntityRegisterEntry
    {
        [SerializeField] private int runtimeId;
        [SerializeField] private string name = null;
        [SerializeField] private string addr = null;
        [SerializeField] private int pool_count = -1;
        [SerializeField] private Sprite asset;
        [SerializeField] private int poolId;
        [SerializeField] private GameObject template;

        public override int Id { get => runtimeId; set => runtimeId = value; }
        public override string RegisterName => name;
        public string Addr => addr;
        public int PoolCount => pool_count;
        public int PoolId => poolId;


        public Sprite Asset
        {
            get => asset;
            set
            {
                if (asset && value)
                {
                    throw new InvalidOperationException("不可重复对需加载资源赋值");
                }

                asset = value;
            }
        }

        public GameObject Template => template;

        public override Entity Instantiate()
        {
            EntityBullet result;
            if (PoolCount < 0)
            {
                result = UnityEngine.Object.Instantiate(Template).GetComponent<EntityBullet>();
                result.poolPtr = -1;
            }
            else
            {
                var myPool = GameManager.Pool.Pools[PoolId];
                var ptr = myPool.Get();
                var go = myPool.Container[ptr];
                var b = go.GetComponent<EntityBullet>();
                b.poolPtr = ptr;
                result = b;
            }

            return result;
        }

        public override void Destroy(Entity instance)
        {
            var bullet = (EntityBullet) instance;
            if (PoolCount < 0)
            {
                base.Destroy(instance);
            }
            else
            {
                if (bullet.poolPtr < 0)
                {
                    throw new InvalidOperationException($"{RegisterName}:{PoolId}不能回收无id的子弹{instance}");
                }

                GameManager.Pool.Return(PoolId, bullet.poolPtr);
                bullet.poolPtr = -1;
            }
        }

        public override void PerProcess() { GameManager.Load.Request<Sprite>(Addr, sprite => Asset = sprite); }

        public override void Process()
        {
            var bgo = new GameObject(RegisterName);
            var bgoSprite = bgo.AddComponent<SpriteRenderer>();
            bgoSprite.sprite = Asset;
            bgo.AddComponent<EntityBullet>();
            poolId = GameManager.Pool.Allocate(bgo, RegisterName, PoolCount, newId => poolId = newId);
            template = bgo;
            template.SetActive(false);
        }

        public override bool Check(out string info)
        {
            var reason = string.Empty;
            bool result = true;
            if (!Asset)
            {
                reason += $"未成功加载资源{Addr},忽略";
                result = false;
            }

            if (!GameManager.Pool.IsAllocated(RegisterName))
            {
                reason += $" 未成功分配资源池{RegisterName},忽略";
                result = false;
            }

            if (!Template)
            {
                reason += $" {RegisterName}未创建子弹模板,忽略";
                result = false;
            }

            info = reason;
            return result;
        }
    }
}