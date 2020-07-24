using UnityEngine;

namespace KSGFK
{
    /// <summary>
    /// 活着的,有血量的生物
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class EntityLiving : Entity
    {
        [SerializeField] protected ulong maxHealth = ulong.MaxValue;
        [SerializeField] protected ulong nowHealth;
        private Rigidbody2D _rigid;
        private Collider2D _coll;

        public ulong MaxHealth
        {
            get => maxHealth;
            set => maxHealth = Helper.SingleAssign(value, maxHealth != ulong.MaxValue);
        }

        public ulong NowHealth { get => nowHealth; set => nowHealth = value; }
        public Rigidbody2D Rigid => _rigid;
        public Collider2D Coll => _coll;

        public override void OnSpawn()
        {
            nowHealth = maxHealth;
            _rigid = GetComponent<Rigidbody2D>();
            _coll = GetComponent<Collider2D>();
        }
    }
}