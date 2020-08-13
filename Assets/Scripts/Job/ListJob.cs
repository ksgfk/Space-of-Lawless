// using System;
// using System.Collections.Generic;
// using MPipeline;
// using Unity.Collections;
//
// namespace KSGFK
// {
//     public abstract unsafe class ListJob<T> : JobWrapper where T : unmanaged
//     {
//         private NativeList<T> _dataList;
//         private readonly List<JobInfo> _jobInfos;
//
//         protected NativeList<T> DataList => _dataList;
//
//         protected ListJob()
//         {
//             _dataList = new NativeList<T>(0, Allocator.Persistent);
//             _jobInfos = new List<JobInfo>();
//         }
//
//         public override JobInfo<TOutput> AddValue<TInput, TOutput>(TInput value)
//         {
//             AddValue()
//         }
//
//         protected abstract T TypeConvert<TInput>(in TInput input) where TInput : struct;
//         
//         private JobInfo<T> AddValue(T value)
//         {
//             if (!(value is T data)) throw new ArgumentException();
//             var index = _dataList.Length;
//             _dataList.Add(data);
//             _jobInfos.Add(new JobInfo<T>(this, index));
//             return _jobInfos[index].Cast<T>();
//         }
//
//         public override void* GetValue(int index) { return _dataList.unsafePtr + index; }
//
//         public override void RemoveValue(int index)
//         {
//             _dataList.Remove(index);
//
//             var lastIndex = _jobInfos.Count - 1;
//             _jobInfos[index].SetIndex(-1);
//             _jobInfos[index] = _jobInfos[lastIndex];
//             _jobInfos.RemoveAt(lastIndex);
//             _jobInfos[index].SetIndex(index);
//         }
//
//         public override void Dispose()
//         {
//             _dataList.Dispose();
//             _jobInfos.Clear();
//         }
//     }
// }