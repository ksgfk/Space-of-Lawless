using UnityEngine;
using UnityEngine.InputSystem;

namespace KSGFK
{
    public abstract class ShipModuleWeapon : ShipModule
    {
        [SerializeField] private float damage = 0;
        [SerializeField] private float attackSpeed = 0;

        public float Damage { get => damage; set => damage = value; }
        public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }

        public abstract void Fire();

        public void OnInputCallbackFire(InputAction.CallbackContext ctx) { Fire(); }
    }
}