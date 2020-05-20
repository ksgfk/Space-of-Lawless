using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public class EntryShipWeapon : EntryShipModule
    {
        [SerializeField] private string addr = null;
        [SerializeField] private GameObject prefab = null;
        [SerializeField] private float damage = 0;
        [SerializeField] private float atk_speed = 0;

        public string Addr => addr;
        public float Damage => damage;
        public float AttackSpeed => atk_speed;
        public GameObject Prefab { get => prefab; set => prefab = Helper.SingleAssign(value, prefab && value); }

        public override void PerProcess() { GameManager.Load.Request<GameObject>(Addr, sprite => Prefab = sprite); }

        public override void Process() { }

        public override bool Check(out string info)
        {
            var result = Helper.CheckResource(Prefab, Addr, out var reason);
            if (result)
            {
                if (!Prefab.TryGetComponent<ShipModuleWeapon>(out _))
                {
                    reason = $"预制体{addr}不存在{typeof(ShipModuleWeapon).FullName}组件,忽略";
                }
            }

            info = reason;
            return result;
        }

        protected override ShipModule InstantiateBehavior()
        {
            var go = UnityEngine.Object.Instantiate(Prefab);
            go.name = $"{RegisterName}:{RuntimeId}";
            var weapon = go.GetComponent<ShipModuleWeapon>();
            weapon.Damage = Damage;
            weapon.AttackSpeed = AttackSpeed;
            return weapon;
        }
    }
}