using UnityEngine;

namespace KSGFK
{
    /// <summary>
    /// 子弹基类
    /// </summary>
    public abstract class EntityBullet : Entity
    {
        /// <summary>
        /// 用于索引对象池中该GO位置的Id
        /// </summary>
        [SerializeField] private int _poolObjectId = -1;

        [SerializeField] protected Entity _launcher;

        /// <summary>
        /// 用于索引对象池中该GO位置的Id
        /// </summary>
        public int PoolObjectId => _poolObjectId;
        
        public void Launch(Entity launcher,Vector2 direction, Vector2 startPos, float speed)
        {
            _launcher = launcher;
            Launch(direction, startPos, speed);
        }

        protected abstract void Launch(Vector2 direction, Vector2 startPos, float speed);

        public void SetPoolObjectId(int id) { _poolObjectId = id; }
    }
}