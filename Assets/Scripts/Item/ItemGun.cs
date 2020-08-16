using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public struct GunInfo
    {
        public float Damage;
        public float RateOfFire;
        public int MagazineCapacity;
        public string BulletName;
    }

    [DisallowMultipleComponent]
    public class ItemGun : Item
    {
        public Vector2 bulletInitPos;
        [SerializeField] private GunInfo _info;
        [SerializeField] private int _bulletId = -1;

        public GunInfo CoreInfo => _info;

        public override void OnUse(EntityLiving user)
        {
            World world = GameManager.Instance.World;
            var bullet = world.SpawnEntity<EntityBullet>(_bulletId);
            Vector2 userPos = user.transform.position;
            Vector2 gunPos = transform.position;
            gunPos += bulletInitPos;
            bullet.Launch(gunPos - userPos, gunPos, 5, 5);
        }

        public override void OnCreate()
        {
            base.OnCreate();
            _bulletId = GameManager.Instance.Register.Entity[_info.BulletName].RuntimeId;
        }

        public void SetGunInfo(in GunInfo info) { _info = info; }
    }
}