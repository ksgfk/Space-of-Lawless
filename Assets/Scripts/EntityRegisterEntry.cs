namespace KSGFK
{
    public abstract class EntityRegisterEntry : IStageProcessEntry
    {
        public abstract int Id { get; set; }

        public abstract string RegisterName { get; }

        public abstract Entity Instantiate();

        public virtual void Destroy(Entity instance) { UnityEngine.Object.Destroy(instance.gameObject); }

        public abstract void PerProcess();

        public abstract void Process();

        public abstract bool Check(out string info);
    }
}