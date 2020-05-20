using UnityEngine;

namespace KSGFK
{
    public abstract class ShipModule : IdentityObject
    {
        [SerializeField] private EntityShip baseShip = null;

        public EntityShip BaseShip { get => baseShip; set => baseShip = Helper.SingleAssign(value, baseShip); }

        public virtual void OnAddToShip() { }
    }
}