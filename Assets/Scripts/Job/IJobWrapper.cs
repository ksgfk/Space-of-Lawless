using System;

namespace KSGFK
{
    public interface IJobWrapper : IDisposable
    {
        void OnUpdate();
    }
}