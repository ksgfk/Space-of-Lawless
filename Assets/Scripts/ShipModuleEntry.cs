namespace KSGFK
{
    public abstract class ShipModuleEntry : IStageProcessEntry
    {
        public abstract int Id { get; set; }

        public abstract string RegisterName { get; }

        public abstract IShipModule Instantiate();

        public virtual void Destroy(IShipModule instance) { UnityEngine.Object.Destroy(instance.BaseGameObject); }

        public abstract void PerProcess();

        public abstract void Process();

        public abstract bool Check(out string info);
    }
}