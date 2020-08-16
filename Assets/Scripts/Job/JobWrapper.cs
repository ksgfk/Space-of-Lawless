using System;
using Unity.Jobs;

namespace KSGFK
{
    public abstract class JobWrapper : IDisposable
    {
        public abstract JobHandle OnUpdate();

        public virtual void AfterUpdate() { }

        public abstract void Dispose();
    }
}