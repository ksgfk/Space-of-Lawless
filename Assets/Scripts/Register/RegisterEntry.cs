using System;

namespace KSGFK
{
    public abstract class RegisterEntry : IComparable<RegisterEntry>
    {
        private int _runtimeId = -1;

        public int RuntimeId => _runtimeId;
        public abstract string RegisterName { get; }

        public void Remap(int runtimeId)
        {
            if (_runtimeId <= -1)
            {
                _runtimeId = runtimeId;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public int CompareTo(RegisterEntry other)
        {
            return string.Compare(RegisterName, other.RegisterName, StringComparison.Ordinal);
        }

        public abstract bool Check(out string reason);
    }
}