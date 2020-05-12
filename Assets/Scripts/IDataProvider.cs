namespace KSGFK
{
    public interface IDataProvider<T> : IDataProvider where T : unmanaged
    {
        void PostUpdate();
    }

    public interface IDataProvider
    {
        int DataId { get; set; }

        IJobWrapper JobWrapper { get; set; }
    }
}