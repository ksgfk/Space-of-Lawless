using System;

// namespace KSGFK
// {
//     [Serializable]
//     public class EntryItemGun : EntryItem
//     {
//         [ReflectionInject] private float damage = -1;
//         [ReflectionInject] private float firing_rate = -1;
//         [ReflectionInject] private int capacity = -2;
//
//         public override bool Check(out string info)
//         {
//             var res = Helper.CheckResource(Prefab, addr, out var resInfo);
//             var com = Helper.CheckComponent<ItemGun>(Prefab, out var comInfo);
//             info = $"[{resInfo}|{comInfo}]";
//             return res && com;
//         }
//
//         protected override Entity SpawnEntity()
//         {
//             var result = (ItemGun) base.SpawnEntity();
//             result.Damage = damage;
//             result.FiringRate = firing_rate;
//             result.Capacity = capacity;
//             return result;
//         }
//     }
// }