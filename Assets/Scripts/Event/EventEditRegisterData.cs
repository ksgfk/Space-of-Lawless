using System;

namespace KSGFK
{
    public class EventEditRegisterData : EventArgs
    {
        public RegisterDataCollection Collection { get; }

        public EventEditRegisterData(RegisterDataCollection collection) { Collection = collection; }
    }
}