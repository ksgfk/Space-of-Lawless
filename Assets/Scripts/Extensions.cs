using System;
using System.Collections.Generic;
using System.Reflection;

namespace KSGFK
{
    public static class Extensions
    {
        public static void Swap<T>(this IList<T> list, int l, int r)
        {
            var t = list[l];
            list[l] = list[r];
            list[r] = t;
        }

        public static int GetLastIndex<T>(this IList<T> list) { return list.Count - 1; }

        public static void RemoveSwapLast<T>(this IList<T> list, int index)
        {
            var last = list.GetLastIndex();
            list.Swap(index, last);
            list.RemoveAt(last);
        }

        public static IEnumerable<FieldInfo> GetAllFields(this Type type, BindingFlags flags)
        {
            var loopType = type;
            var list = new List<FieldInfo>();
            while (loopType != null && loopType != typeof(object))
            {
                var fields = loopType.GetFields(flags);
                list.AddRange(fields);
                loopType = loopType.BaseType;
            }

            return list;
        }
    }
}