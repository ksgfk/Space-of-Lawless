using UnityEngine;

namespace KSGFK
{
    public abstract class ShipModuleEngine : ShipModule
    {
        [SerializeField] private float maxMoveSpeed = -1;
        [SerializeField] private float maxRotateSpeed = -1;
        [SerializeField] protected bool canMove;

        public float MaxMoveSpeed
        {
            get => maxMoveSpeed;
            set => maxMoveSpeed = Helper.SingleAssign(value, maxMoveSpeed >= 0);
        }

        public float MaxRotateSpeed
        {
            get => maxRotateSpeed;
            set => maxRotateSpeed = Helper.SingleAssign(value, maxRotateSpeed >= 0);
        }

        public virtual bool CanMove { get => canMove; set => canMove = value; }

        public abstract void SetMoveDirection(Vector2 direction);

        public abstract void SetRotateDelta(Vector2 delta);

        public virtual void Move() { }
    }
}