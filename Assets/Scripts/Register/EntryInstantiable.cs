namespace KSGFK
{
    public abstract class EntryInstantiable<T> : RegisterEntry where T : IRuntimeIdentity
    {
        protected abstract T Construct();

        public abstract void Destroy(T instance);

        public T Instantiate()
        {
            var instance = Construct();
            instance.SetId(RuntimeId);
            return instance;
        }
    }
}