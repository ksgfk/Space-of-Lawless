using System;
using System.Collections;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public struct GunInfo
    {
        public float Damage;
        public float RateOfFire;
        public float Reload;
        public int MagazineCapacity;
        public string EntityBulletName;
        public AmmoType UsableAmmo;
    }

    [DisallowMultipleComponent]
    public class ItemGun : Item
    {
        public Transform barrelStart;
        public Transform muzzle;
        [SerializeField] private GunInfo _info;
        [SerializeField] private int _bulletId = -1;
        [SerializeField] private float _lastFireTime;
        [SerializeField] private int _nowMagCap;
        [SerializeField] private bool _isReload;
        private WaitForSeconds _waitReload;

        public GunInfo CoreInfo => _info;

        public override void OnUse(EntityLiving user)
        {
            CheckAmmo(user);
            if (CanFire())
            {
                Fire(user);
                AfterFire();
            }
        }

        private void CheckAmmo(EntityLiving user)
        {
            if (_nowMagCap != 0)
            {
                return;
            }

            if (_isReload)
            {
                return;
            }

            Inventory inv = user.Inventory;
            var reloadAmmo = inv.FindFirst(i => i is ItemBullet ib && ib.ammoType.Equals(_info.UsableAmmo));
            if (reloadAmmo != -1)
            {
                StartCoroutine(Reload(inv, reloadAmmo));
                _isReload = true;
            }
        }

        private IEnumerator Reload(Inventory inv, int pos)
        {
            yield return _waitReload;
            var ammo = inv[pos];
            _isReload = false;
            var canUseAmmo = ammo.NowStack >= _info.MagazineCapacity ? _info.MagazineCapacity : ammo.NowStack;
            ammo.NowStack -= canUseAmmo;
            _nowMagCap += canUseAmmo;
        }

        private bool CanFire()
        {
            var nowTime = Time.time;
            if (_isReload)
            {
                return false;
            }

            if (_nowMagCap <= -2 || _nowMagCap == 0) //没弹药了
            {
                return false;
            }

            var nextFireTime = _lastFireTime + _info.RateOfFire;
            return nowTime >= nextFireTime;
        }

        private void Fire(EntityLiving user)
        {
            World world = GameManager.Instance.World;
            var bullet = world.SpawnEntity<EntityBullet>(_bulletId);
            Vector2 startPos = barrelStart.position;
            Vector2 endPos = muzzle.position;
            bullet.Launch(user, endPos - startPos, startPos, 5);
        }

        private void AfterFire()
        {
            _lastFireTime = Time.time;
            if (_nowMagCap > 0)
            {
                _nowMagCap--;
            }
        }

        public override void OnCreate()
        {
            base.OnCreate();
            _bulletId = GameManager.Instance.Register.Entity[_info.EntityBulletName].RuntimeId;
            _nowMagCap = _info.MagazineCapacity;
            _isReload = false;
            _waitReload = new WaitForSeconds(_info.Reload);
        }

        public void SetGunInfo(in GunInfo info) { _info = info; }
    }
}