using System;

// namespace KSGFK
// {
//     [Serializable]
//     public class EntryJob : IStageProcessEntry
//     {
//         private int _runtimeId = int.MinValue;
//         [ReflectionInject] private string name = null;
//         [ReflectionInject] private string full_type_name = null;
//         private Type _jobType;
//
//         public int RuntimeId
//         {
//             get => _runtimeId;
//             set => _runtimeId = Helper.SingleAssign(value, _runtimeId != int.MinValue);
//         }
//
//         public string RegisterName => name;
//         public string FullTypeName => full_type_name;
//         public Type JobType => _jobType;
//
//         public void PerProcess() { }
//
//         public void Process()
//         {
//             try
//             {
//                 _jobType = Type.GetType(FullTypeName);
//             }
//             catch (Exception)
//             {
//                 _jobType = null;
//             }
//         }
//
//         public bool Check(out string info)
//         {
//             var isSuccessFindType = _jobType != null;
//             var resultInfo = isSuccessFindType ? string.Empty : $"不存在类型:{FullTypeName}";
//             if (isSuccessFindType)
//             {
//                 var isAssigned = typeof(IJobWrapper).IsAssignableFrom(JobType);
//                 resultInfo += isAssigned ? string.Empty : $" 类型{FullTypeName}没有实现{typeof(IJobWrapper)}接口";
//                 isSuccessFindType = isAssigned;
//             }
//
//             info = resultInfo;
//             return isSuccessFindType;
//         }
//     }
// }