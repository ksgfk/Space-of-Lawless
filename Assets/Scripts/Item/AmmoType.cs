using System;

namespace KSGFK
{
    public readonly struct AmmoType : IEquatable<AmmoType>
    {
        private readonly int _value;

        public AmmoType(int value)
        {
            if ((value & (value - 1)) != 0)
            {
                throw new ArgumentException();
            }

            _value = value;
        }

        public bool Equals(AmmoType other) { return _value == other._value; }

        public override bool Equals(object obj) { return obj is AmmoType other && Equals(other); }

        public override int GetHashCode() { return _value.GetHashCode(); }

        public static AmmoType HgAmmo => new AmmoType(0b0001);
        public static AmmoType ArAmmo => new AmmoType(0b0010);
        public static AmmoType TntAmmo => new AmmoType(0b0100);
    }
}