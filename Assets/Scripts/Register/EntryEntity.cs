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
            AfterCallOnRemoveFromWorld(instance);
        }

        protected virtual void AfterCallOnRemoveFromWorld(Entity entity)
        {
            UnityEngine.Object.Destroy(entity.gameObject);
        }

        public abstract Task PreProcess();

        public virtual void Process() { }
    }
}