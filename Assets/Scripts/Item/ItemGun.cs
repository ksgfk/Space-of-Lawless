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
    }

    [DisallowMultipleComponent]
    public class ItemGun : Item
    {
        public Vector2 bulletInitPos;
        [SerializeField] private GunInfo _info;

        public GunInfo CoreInfo => _info;

        public void SetGunInfo(in GunInfo info) { _info = info; }
    }
}