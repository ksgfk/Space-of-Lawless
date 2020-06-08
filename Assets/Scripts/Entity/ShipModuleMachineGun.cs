using System;
using UnityEngine;
using Random = UnityEngine.Random;

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

        public override void Fire()
        {
            for (int i = 0; i < 10; i++)
            {
                var bullet = GameManager.Entity.SpawnEntity<EntityBullet>(bulletRuntimeId);
                bullet.Launch(
                    (BaseShip.transform.up + new Vector3(Random.Range(-10, 10), Random.Range(-10, 10))).normalized,
                    transform.position,
                    Damage,
                    Random.Range(1, 10));
            }
        }
    }
}