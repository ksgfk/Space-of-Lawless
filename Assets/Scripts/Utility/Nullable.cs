using System;

namespace KSGFK
{
    /// <summary>
    /// 可空引用类型
    /// Unity什么时候引入C#8啊啊啊
    /// </summary>
    public readonly struct Nullable<T> : IEquatable<Nullable<T>> where T : class
    {
        public bool HasValue => Value != null;

        public T Value { get; }

        public Nullable(T value) { Value = value; }

        public bool Equals(T obj)
        {
            if (HasValue)
            {
                return Value.Equals(obj);
            }

            return obj == null;
        }

        public bool Equals(Nullable<T> other) { return Equals(other.Value); }

        public override bool Equals(object obj) { return obj is Nullable<T> other && Equals(other); }

        public override int GetHashCode() { return HasValue ? Value.GetHashCode() : 0; }

        public static implicit operator T(Nullable<T> value) { return value.Value; }
    }
}