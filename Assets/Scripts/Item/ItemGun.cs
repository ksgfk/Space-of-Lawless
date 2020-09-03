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
        public Transform barrelStart;
        public Transform muzzle;
        public AmmoTypes usableAmmo;
        [SerializeField] private GunInfo _info;
        [SerializeField] private int _bulletId = -1;
        [SerializeField] private float _lastFireTime;
        [SerializeField] private int _nowMagCap;

        public GunInfo CoreInfo => _info;

        public override void OnUse(EntityLiving user)
        {
            if (_nowMagCap == 0)
            {
                Inventory inv = user.Inventory;
                var itemBullet = inv.FindFirst(i => i is ItemBullet ib && ib.ammoType == usableAmmo);
                if (itemBullet)
                {
                    var b = itemBullet.NowStack;
                    var willIn = b >= _info.MagazineCapacity ? _info.MagazineCapacity : b;
                    itemBullet.NowStack -= willIn;
                    _nowMagCap += willIn;
                }
            }

            var nowTime = Time.time;
            var nextFireTime = _lastFireTime + _info.RateOfFire;
            if (nowTime < nextFireTime)
            {
                return;
            }

            if (_nowMagCap <= -2 || _nowMagCap == 0)
            {
                return;
            }

            World world = GameManager.Instance.World;
            var bullet = world.SpawnEntity<EntityBullet>(_bulletId);
            Vector2 startPos = barrelStart.position;
            Vector2 endPos = muzzle.position;
            bullet.Launch(user, endPos - startPos, startPos, 5);

            _lastFireTime = nowTime;
            if (_nowMagCap > 0)
            {
                _nowMagCap--;
            }
        }

        public override void OnCreate()
        {
            base.OnCreate();
            _bulletId = GameManager.Instance.Register.Entity[_info.BulletName].RuntimeId;
            _nowMagCap = _info.MagazineCapacity;
        }

        public void SetGunInfo(in GunInfo info) { _info = info; }
    }
}