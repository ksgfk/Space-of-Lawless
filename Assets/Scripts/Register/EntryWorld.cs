using System;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace KSGFK
{
    [Serializable]
    public struct WorldInfo
    {
        public readonly string Name;
        public readonly string Addr;

        public WorldInfo(string name, string addr)
        {
            Name = name;
            Addr = addr;
        }
    }

    public class EntryWorld : RegisterEntry
    {
        private readonly WorldInfo _info;

        public override string RegisterName => _info.Name;
        public string AssetAddr => _info.Addr;

        public EntryWorld(WorldInfo info) { _info = info; }

        public override bool Check(out string info)
        {
            var res = Helper.IsResourceExist<SceneInstance>(AssetAddr);
            info = res ? string.Empty : $"不存在场景{AssetAddr}";
            return res;
        }
    }
}