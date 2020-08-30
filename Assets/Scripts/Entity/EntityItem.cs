using UnityEngine;

namespace KSGFK
{
    public class EntityItem : Entity
    {
        [SerializeField] private Item _hold;
        [SerializeField] private Entity _thrower;
        private CircleCollider2D _coll;

        public Item Hold
        {
            get => _hold;
            set
            {
                if (!value)
                {
                    GameManager.Instance.World.Value.DestroyEntity(this);
                    return;
                }

                _hold = value;
                var trans = _hold.transform;
                trans.SetParent(transform);
                trans.localPosition = Vector3.zero;
                trans.localRotation = Quaternion.identity;
            }
        }

        public Entity Thrower => _thrower;
        public CircleCollider2D Coll => _coll;

        public void SetThrower(Entity thrower) { _thrower = thrower; }

        public override void OnSpawn() { _coll = GetComponent<CircleCollider2D>(); }
    }
}