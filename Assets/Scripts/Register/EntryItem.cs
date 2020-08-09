using UnityEngine;

namespace KSGFK
{
    public class EntryItem : EntryEntityLiving
    {
        // [ReflectionInject] protected string addr = null;
        // private GameObject _prefab;
        //
        // public GameObject Prefab => _prefab;
        //
        // public override void PerProcess()
        // {
        //     GameManager.Instance.Load.Request<GameObject>(addr, handle => _prefab = Helper.GetAsyncOpResult(handle));
        // }
        //
        // public override bool Check(out string info)
        // {
        //     var res = Helper.CheckResource(_prefab, addr, out var resInfo);
        //     var com = Helper.CheckComponent<Item>(_prefab, out var comInfo);
        //     info = $"[{resInfo}|{comInfo}]";
        //     return res && com;
        // }
        //
        // protected override Entity SpawnEntity() { return Object.Instantiate(Prefab).GetComponent<Item>(); }
        public EntryItem(EntityLivingInfo info) : base(info) { }
    }
}