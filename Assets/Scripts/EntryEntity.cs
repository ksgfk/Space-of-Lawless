namespace KSGFK
{
    public abstract class EntryEntity : EntryIdentity<Entity>
    {
        protected abstract Entity SpawnEntity();

        protected abstract void DestroyEntity(Entity instance);

        protected sealed override Entity InstantiateBehavior()
        {
            var result = SpawnEntity();
            result.OnSpawn();
            return result;
        }

        public override void Destroy(Entity instance)
        {
            instance.OnRemoveFromWorld();
            DestroyEntity(instance);
        }
    }
}