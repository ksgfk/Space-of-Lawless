using System;

namespace KSGFK
{
    public interface IJobWrapper : IDisposable
    {
        string Name { get; }

        void OnUpdate(float deltaTime);
    }
}