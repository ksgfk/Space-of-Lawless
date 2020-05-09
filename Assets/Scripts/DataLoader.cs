using System;

namespace KSGFK
{
    public abstract class DataLoader
    {
        public abstract IAsyncHandleWrapper StartLoad(Type type, string path);
    }
}