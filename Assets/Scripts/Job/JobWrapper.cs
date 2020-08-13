// using System;
//
// namespace KSGFK
// {
//     public abstract unsafe class JobWrapper : IDisposable
//     {
//         public abstract JobInfo<TOutput> AddValue<TInput, TOutput>(TInput value)
//             where TInput : struct
//             where TOutput : unmanaged;
//
//         public abstract void RemoveValue(int index);
//
//         public abstract void* GetValue(int index);
//
//         public abstract void OnUpdate();
//
//         public abstract void Dispose();
//     }
// }