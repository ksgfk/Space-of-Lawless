using UnityEngine;

namespace KSGFK
{
    [DisallowMultipleComponent]
    public class ItemGun : Item
    {
        public Vector2 bulletInitPos;
        [SerializeField] private float _damage = -1;
        [SerializeField] private float _firingRate = -1;
        [SerializeField] private int _capacity = -2;
        
        public float Damage
        {
            get => _damage;
            set => _damage = Helper.SingleAssign(value, MathExt.IsEqual(value, -1f));
        }

        public float FiringRate
        {
            get => _firingRate;
            set => _firingRate = Helper.SingleAssign(value, MathExt.IsEqual(value, -1f));
        }

        public int Capacity
        {
            get => _capacity;
            set => _capacity = Helper.SingleAssign(value, MathExt.IsEqual(value, -1f));
        }
    }
}