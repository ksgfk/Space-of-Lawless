using System;

namespace KSGFK
{
    public interface IJobWrapper : IDisposable
    {
        int RuntimeId { get; set; }

        void OnUpdate(float deltaTime);

        void AddData(IDataProvider item);

        void RemoveData(int runtimeId);
    }
}