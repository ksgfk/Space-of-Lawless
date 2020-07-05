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
        [SerializeField] private int poolObjectId = -1;

        /// <summary>
        /// 用于索引对象池中该GO位置的Id
        /// </summary>
        public int PoolObjectId { get => poolObjectId; internal set => poolObjectId = value; }

        /// <summary>
        /// 发射
        /// </summary>
        /// <param name="direction">方向</param>
        /// <param name="startPos">起始位置</param>
        /// <param name="speed">速度</param>
        /// <param name="duration">存活时间</param>
        public abstract void Launch(Vector2 direction, Vector2 startPos, float speed, float duration);
    }
}