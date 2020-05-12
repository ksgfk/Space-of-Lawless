namespace KSGFK
{
    public abstract class EntityRegisterEntry<T> : IStageProcessEntry where T : Entity
    {
        public abstract int Id { get; set; }

        public abstract string RegisterName { get; }

        public abstract T Instantiate();

        public virtual void Destroy(T instance) { UnityEngine.Object.Destroy(instance.gameObject); }

        public abstract void PerProcess();

        public abstract void Process();

        public abstract bool Check(out string info);
    }
}