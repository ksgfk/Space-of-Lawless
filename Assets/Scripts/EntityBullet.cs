using UnityEngine;

namespace KSGFK
{
    public abstract class EntityBullet : Entity
    {
        [SerializeField] private int poolObjectId = -1;

        public int PoolObjectId { get => poolObjectId; internal set => poolObjectId = value; }

        public abstract void Launch(Vector2 direction, Vector2 startPos, float speed, float duration);
    }
}