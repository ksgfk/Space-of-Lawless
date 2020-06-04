using UnityEngine;

namespace KSGFK
{
    public abstract class ShipModuleWeapon : ShipModule
    {
        [SerializeField] private float damage = 0;
        [SerializeField] private float attackSpeed = 0;
        [SerializeField] private bool canFire = false;

        public float Damage { get => damage; set => damage = value; }
        public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
        public bool CanFire { get => canFire; set => canFire = value; }

        public abstract void Fire();

        private void Update()
        {
            if (canFire)
            {
                Fire();
            }
        }
    }
}