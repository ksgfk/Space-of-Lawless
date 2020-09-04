using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace KSGFK
{
    public class ItemInfo : EntryBaseInfo
    {
        public int MaxStack { get; set; }
        public float CollideRadius { get; set; }
    }

    public class EntryItem : EntryInstantiable<Item>, IStageProcess
    {
        private readonly ItemInfo _baseInfo;
        protected GameObject MPrefab;

        public float CollideRadius => _baseInfo.CollideRadius;
        public int MaxStack => _baseInfo.MaxStack;
        public override string RegisterName => _baseInfo.Name;
        public ItemInfo BaseInfo => _baseInfo;
        public GameObject Prefab => MPrefab;

        public EntryItem(ItemInfo baseInfo) { _baseInfo = baseInfo; }

        protected virtual Item CreateItem()
        {
            var go = UnityEngine.Object.Instantiate(Prefab);
            return go.GetComponent<Item>();
        }

        protected virtual void DestroyItem(Item instance) { UnityEngine.Object.Destroy(instance.gameObject); }

        protected sealed override Item Construct()
        {
            var item = CreateItem();
            item.SetMaxStack(MaxStack);
            item.SetCollideRadius(CollideRadius);
            item.OnCreate();
            return item;
        }

        public override void Destroy(Item instance)
        {
            instance.OnDestroyItem();
            DestroyItem(instance);
        }

        public virtual Task PreProcess()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(BaseInfo.Addr);
            handle.Completed += h => MPrefab = Helper.GetAsyncOpResult(h);
            return handle.Task;
        }

        public virtual void Process() { }

        public override bool Check(out string reason)
        {
            var res = Helper.CheckResource(MPrefab, BaseInfo.Addr, out var resInfo);
            var com = Helper.CheckComponent<Item>(MPrefab, out var comInfo);
            reason = $"{resInfo}|{comInfo}";
            return res && com;
        }
    }
}