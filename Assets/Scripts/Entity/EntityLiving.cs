using UnityEngine;

namespace KSGFK
{
    /// <summary>
    /// 活着的,有血量的生物
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class EntityLiving : Entity
    {
        [SerializeField] protected ulong _maxHealth = ulong.MaxValue;
        [SerializeField] protected ulong _nowHealth;
        private Rigidbody2D _rigid;
        private Collider2D _coll;
        [SerializeField] private Inventory _inventory = null;

        public ulong MaxHealth => _maxHealth;

        public ulong NowHealth { get => _nowHealth; set => _nowHealth = value; }
        public Rigidbody2D Rigid => _rigid;
        public Collider2D Coll => _coll;
        public Nullable<Inventory> Inventory => new Nullable<Inventory>(_inventory);

        public override void OnSpawn()
        {
            _nowHealth = _maxHealth;
            _rigid = GetComponent<Rigidbody2D>();
            _coll = GetComponent<Collider2D>();
            if (_inventory)
            {
                _inventory.Init(this);
            }
        }

        public void SetMaxHealth(ulong health) { _maxHealth = health; }
    }
}