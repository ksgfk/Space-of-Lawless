using System;

namespace KSGFK
{
    public interface IDataProvider<T> : IDataProvider where T : unmanaged
    {
        T NewData();

        void PostUpdate();
    }

    public interface IDataProvider
    {
        int DataId { get; set; }

        IJobWrapper JobWrapper { get; }

        Type DataType { get; }
    }
}