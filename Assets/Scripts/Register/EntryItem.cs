using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace KSGFK
{
    [Serializable]
    public readonly struct ItemInfo
    {
        public readonly string Name;
        public readonly string Addr;
        public readonly int MaxStack;
        public readonly float CollideRadius;

        public ItemInfo(string name, string addr, int maxStack, float collideRadius)
        {
            Name = name;
            Addr = addr;
            MaxStack = maxStack;
            CollideRadius = collideRadius;
        }
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