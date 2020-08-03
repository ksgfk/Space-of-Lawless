using System;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace KSGFK
{
    [Serializable]
    public class EntryWorld : IStageProcessEntry
    {
        private int _runtimeId = int.MinValue;
        [ReflectionInject] private string name = null;
        [ReflectionInject] private string addr = null;

        public int RuntimeId
        {
            get => _runtimeId;
            set => _runtimeId = Helper.SingleAssign(value, _runtimeId != int.MinValue);
        }

        public string RegisterName => name;
        public string Addr => addr;

        public void PerProcess() { }

        public void Process() { }

        public bool Check(out string info)
        {
            var res = Helper.IsResourceExist<SceneInstance>(Addr);
            info = res ? string.Empty : $"不存在场景{Addr}";
            return res;
        }
    }
}