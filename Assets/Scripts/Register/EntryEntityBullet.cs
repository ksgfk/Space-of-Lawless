using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace KSGFK
{
    public class EntryEntityBullet : EntryEntity
    {
        private readonly EntryBaseInfo _info;
        private GameObject _prefab;
        private int _poolId;

        public override string RegisterName => _info.Name;
        public GameObject Prefab => _prefab;
        public string AssetAddr => _info.Addr;
        public int PoolId => _poolId;

        public EntryEntityBullet(EntryBaseInfo info)
        {
            _info = info;
            _poolId = -1;
        }

        public override Task PreProcess()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(_info.Addr);
            handle.Completed += h => _prefab = Helper.GetAsyncOpResult(h);
            return handle.Task;
        }

        public override bool Check(out string reason)
        {
            var res = Helper.CheckResource(_prefab, AssetAddr, out var resInfo);
            var com = Helper.CheckComponent<EntityBullet>(_prefab, out var comInfo);
            reason = $"{resInfo}|{comInfo}";
            return res && com;
        }

        protected override Entity SpawnEntity()
        {
            World world = GameManager.Instance.World;
            if (_poolId == -1)
            {
                _poolId = world.Pool.Allocate(Prefab, RegisterName);
                world.Unload += _ => _poolId = -1;
            }

            var pool = world.Pool[_poolId];
            var objId = pool.Get();
            var bullet = pool.Container[objId].GetComponent<EntityBullet>();
            bullet.SetPoolObjectId(objId);
            return bullet;
        }

        protected override void AfterDestroyEntity(Entity entity)
        {
            World world = GameManager.Instance.World;
            var bullet = (EntityBullet) entity;
            world.Pool.Return(PoolId, bullet.PoolObjectId);
            bullet.SetPoolObjectId(-1);
            bullet.SetId(-1);
        }
    }
}