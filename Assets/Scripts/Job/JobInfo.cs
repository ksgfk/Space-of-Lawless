// using System;
// using MPipeline;
// using UnityEngine;
//
// namespace KSGFK
// {
//     [Serializable]
//     public class JobInfo
//     {
//         [SerializeField] private int _index;
//         public JobWrapper Wrapper { get; }
//         public int Index => _index;
//
//         protected JobInfo(JobWrapper wrapper, int index)
//         {
//             Wrapper = wrapper;
//             _index = index;
//         }
//
//         public void SetIndex(int index) { _index = index; }
//
//         public JobInfo<T> Cast<T>() where T : unmanaged { return (JobInfo<T>) this; }
//
//         public void Release() { Wrapper.RemoveValue(Index); }
//     }
//
//     public class JobInfo<T> : JobInfo where T : unmanaged
//     {
//         public unsafe T* Data => MUnsafeUtility.Cast<T>(Wrapper.GetValue(Index));
//
//         public JobInfo(JobWrapper wrapper, int index) : base(wrapper, index) { }
//     }
// }