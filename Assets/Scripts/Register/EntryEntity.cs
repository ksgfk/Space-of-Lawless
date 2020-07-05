namespace KSGFK
{
    /// <summary>
    /// 实体注册项基类
    /// </summary>
    public abstract class EntryEntity : EntryIdentity<Entity>
    {
        /// <summary>
        /// 实例化实体时调用
        /// </summary>
        protected abstract Entity SpawnEntity();

        /// <summary>
        /// 销毁实体时调用
        /// </summary>
        protected virtual void DestroyEntity(Entity instance) { base.Destroy(instance); }

        protected sealed override Entity InstantiateBehavior()
        {
            var result = SpawnEntity();
            result.OnSpawn();
            return result;
        }

        public sealed override void Destroy(Entity instance)
        {
            instance.OnRemoveFromWorld();
            DestroyEntity(instance);
        }
    }
}