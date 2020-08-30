using System.Threading.Tasks;

namespace KSGFK
{
    public abstract class EntryItem : EntryInstantiable<Item>, IStageProcess
    {
        public abstract float CollideRadius { get; }
        public abstract int MaxStack { get; }

        protected abstract Item CreateItem();

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

        public abstract Task PreProcess();

        public virtual void Process() { }
    }
}