using System;

namespace KSGFK
{
    /// <summary>
    /// 表示该字段将会被反射赋值
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ReflectionInjectAttribute : Attribute
    {
    }
}