using System;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public struct AmmoType : IEquatable<AmmoType>
    {
        [SerializeField] private int _value;

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

        public static implicit operator int(AmmoType ammoType) { return ammoType._value; }

        public static implicit operator AmmoType(int value) { return new AmmoType(value); }

        public static AmmoType HgAmmo => new AmmoType(0b0001);
        public static AmmoType ArAmmo => new AmmoType(0b0010);
        public static AmmoType TntAmmo => new AmmoType(0b0100);
    }

    public class AmmoTypeConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (int.TryParse(text, out var result))
            {
                return new AmmoType(result);
            }

            var type = typeof(AmmoType);
            var field = type.GetProperty(text, BindingFlags.Static | BindingFlags.Public);
            if (field == null)
            {
                throw new ArgumentException();
            }

            return field.GetValue(null);
        }
    }
}