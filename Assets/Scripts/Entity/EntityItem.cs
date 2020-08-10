using UnityEngine;

namespace KSGFK
{
    public class EntityItem : Entity
    {
        [SerializeField] private Item _hold;
        [SerializeField] private Entity _thrower;
        private CircleCollider2D _range;

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

        public void SetThrower(Entity thrower) { _thrower = thrower; }

        public override void OnSpawn()
        {
            _range = GetComponent<CircleCollider2D>();
            _range.radius = 1;
        }
    }
}