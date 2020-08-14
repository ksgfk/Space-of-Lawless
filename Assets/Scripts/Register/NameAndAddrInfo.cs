using System;

namespace KSGFK
{
    [Serializable]
    public readonly struct NameAndAddrInfo
    {
        public readonly string Name;
        public readonly string Addr;

        public NameAndAddrInfo(string name, string addr)
        {
            Name = name;
            Addr = addr;
        }
    }
}