namespace KSGFK
{
    public abstract class ShipModuleEntry : IStageProcessEntry
    {
        public abstract int Id { get; set; }

        public abstract string RegisterName { get; }

        public abstract IShipModule Instantiate();

        public abstract void Destroy(IShipModule instance);

        public abstract void PerProcess();

        public abstract void Process();

        public abstract bool Check(out string info);
    }
}