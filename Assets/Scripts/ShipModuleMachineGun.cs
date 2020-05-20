using System;
using UnityEngine;

namespace KSGFK
{
    public class ShipModuleMachineGun : ShipModuleWeapon
    {
        public string entityBulletName = "normal";
        [SerializeField] private int bulletRuntimeId = -1;

        public override void OnAddToShip()
        {
            var entityEntry = GameManager.Entity.EntityEntry[entityBulletName];
            if (entityEntry is EntryEntityBullet bulletEntry)
            {
                bulletRuntimeId = bulletEntry.RuntimeId;
            }
            else
            {
                throw new ArgumentException($"实体名{entityBulletName}不是子弹{typeof(EntryEntityBullet)}");
            }
        }

        public override void Fire() { GameManager.Entity.SpawnEntity(bulletRuntimeId); }
    }
}