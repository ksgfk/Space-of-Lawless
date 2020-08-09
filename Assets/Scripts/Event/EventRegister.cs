using System;

namespace KSGFK
{
    public class EventRegister<T> : EventArgs where T : RegisterEntry
    {
        public Registry<T> Registry { get; }

        public EventRegister(Registry<T> registry) { Registry = registry; }
    }
}