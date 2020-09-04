using UnityEngine.ResourceManagement.ResourceProviders;

namespace KSGFK
{
    public class EntryWorld : RegisterEntry
    {
        private readonly EntryBaseInfo _info;

        public override string RegisterName => _info.Name;
        public string AssetAddr => _info.Addr;

        public EntryWorld(EntryBaseInfo info) { _info = info; }

        public override bool Check(out string info)
        {
            var res = Helper.IsResourceExist<SceneInstance>(AssetAddr);
            info = res ? string.Empty : $"不存在场景{AssetAddr}";
            return res;
        }
    }
}