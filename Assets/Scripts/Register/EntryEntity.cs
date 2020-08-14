using System.Threading.Tasks;

namespace KSGFK
{
    public abstract class EntryEntity : EntryInstantiable<Entity>, IStageProcess
    {
        protected abstract Entity SpawnEntity();

        protected sealed override Entity Construct()
        {
            var entity = SpawnEntity();
            entity.OnSpawn();
            return entity;
        }

        public sealed override void Destroy(Entity instance)
        {
            instance.OnRemoveFromWorld();
            AfterDestroyEntity(instance);
        }

        protected virtual void AfterDestroyEntity(Entity entity)
        {
            UnityEngine.Object.Destroy(entity.gameObject);
        }

        public virtual Task PreProcess() { return null; }

        public virtual void Process() { }
    }
}