using UnityEngine;

namespace KSGFK
{
    /// <summary>
    /// 活着的,有血量的生物
    /// </summary>
    public class EntityLiving : Entity
    {
        [SerializeField] private ulong _maxHealth = ulong.MaxValue;

        public ulong MaxHealth
        {
            get => _maxHealth;
            set => _maxHealth = Helper.SingleAssign(value, _maxHealth != ulong.MaxValue);
        }
    }
}