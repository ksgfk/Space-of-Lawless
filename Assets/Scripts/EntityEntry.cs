namespace KSGFK
{
    public abstract class EntityEntry<T> : IEntry<T> where T : Entity
    {
        public abstract int Id { get; set; }
        public abstract string RegisterName { get; }

        public abstract T Instantiate();

        public virtual void Destroy(T instance) { UnityEngine.Object.Destroy(instance.gameObject); }
    }
}