using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace KSGFK
{
    /// <summary>
    /// 一个场景是一个世界，管理所有实体的生命周期，包含对象池功能
    /// </summary>
    [DisallowMultipleComponent]
    public class World : MonoBehaviour, IDisposable
    {
        private GameManager _gm;
        private SceneInstance _sceneInstance;
        private LinkedList<Entity> _activeEntity;
        private PoolCenter _pool;
        private List<Entity> _rmCache;
        private HashSet<Entity> _cacheSet;

        /// <summary>
        /// 场景实例
        /// </summary>
        public SceneInstance Scene => _sceneInstance;

        /// <summary>
        /// 活动中实体
        /// </summary>
        public IEnumerable<Entity> ActiveEntity => _activeEntity;

        /// <summary>
        /// 对象池
        /// </summary>
        public PoolCenter Pool => _pool;

        public GameManager GM => _gm;

        /// <summary>
        /// 世界被卸载时触发事件,所有注册了对象池且引用了池ID的地方都必须监听该事件来释放ID
        /// </summary>
        public event Action<World> Unload;

        public virtual void Init(GameManager gm, in SceneInstance sceneInstance)
        {
            _gm = gm;
            _sceneInstance = sceneInstance;
            _activeEntity = new LinkedList<Entity>();
            _pool = new PoolCenter();
            _rmCache = new List<Entity>();
            _cacheSet = new HashSet<Entity>();
        }

        protected virtual Entity SpawnEntity(EntryEntity entry)
        {
            if (entry == null)
            {
                return null;
            }

            var instance = entry.Instantiate();
            AddToActiveEntity(instance);
            return instance;
        }

        /// <summary>
        /// 根据Id来创建实体,如果id不存在则返回null
        /// </summary>
        public Entity SpawnEntity(int id)
        {
            var entry = GetEntityRegistry()[id];
            return SpawnEntity(entry);
        }

        /// <summary>
        /// 根据Id来创建实体,如果id不存在,或者类型转换失败则返回null
        /// </summary>
        public T SpawnEntity<T>(int id) where T : Entity { return SpawnEntity(id) as T; }

        /// <summary>
        /// 根据注册名来创建实体,如果注册名不存在则返回null
        /// </summary>
        public Entity SpawnEntity(string registerName)
        {
            var entry = GetEntityRegistry()[registerName];
            return SpawnEntity(entry);
        }

        /// <summary>
        /// 根据注册名来创建实体,如果注册名不存在,或者类型转换失败则返回null
        /// </summary>
        public T SpawnEntity<T>(string id) where T : Entity { return SpawnEntity(id) as T; }

        /// <summary>
        /// 销毁实体
        /// </summary>
        public void DestroyEntity(Entity entity)
        {
            if (!entity || !entity.IsInWorld)
            {
                Debug.LogWarningFormat("已经被销毁的实体:{0}", entity.RuntimeId);
                return;
            }

            _rmCache.Add(entity);
        }

        public void AfterUpdate()
        {
            _cacheSet.UnionWith(_rmCache);
            foreach (var entity in _cacheSet)
            {
                var entry = GetEntityRegistry()[entity.RuntimeId];
                _activeEntity.Remove(entity.Node);
                entry.Destroy(entity);
            }

            _cacheSet.Clear();
            _rmCache.Clear();
        }

        protected virtual Registry<EntryEntity> GetEntityRegistry() { return _gm.Register.Entity; }

        public void Dispose()
        {
            Unload?.Invoke(this);
            Pool.Dispose();
        }

        protected virtual void AddToActiveEntity(Entity entity)
        {
            LinkedListNode<Entity> node;
            if (entity.Node == null)
            {
                node = new LinkedListNode<Entity>(entity);
                entity.Node = node;
            }
            else
            {
                node = entity.Node;
            }

            _activeEntity.AddLast(node);
        }

        protected virtual Registry<EntryItem> GetItemRegistry() { return _gm.Register.Item; }

        public EntityItem CreateItemInWorld(int itemId, int count, Entity creator = null)
        {
            return CreateItemInWorld(GetItemRegistry()[itemId], count, creator);
        }

        public EntityItem CreateItemInWorld(string itemName, int count, Entity creator = null)
        {
            return CreateItemInWorld(GetItemRegistry()[itemName], count, creator);
        }

        private EntityItem CreateItemInWorld(EntryItem itemEntry, int count, Entity creator)
        {
            if (itemEntry == null)
            {
                return null;
            }

            var item = itemEntry.Instantiate();
            item.NowStack = count;
            return CreateItemInWorld(item, creator);
        }

        public EntityItem CreateItemInWorld(Item item, Entity creator = null)
        {
            if (!item)
            {
                return null;
            }

            if (item.NowStack <= 0)
            {
                return null;
            }

            var ei = _gm.Register.NewEntityItem;
            ei.Hold = item;
            ei.SetThrower(creator);
            AddToActiveEntity(ei);
            return ei;
        }

        public void DestroyItem(Item item)
        {
            if (!item)
            {
                return;
            }

            GetItemRegistry()[item.RuntimeId].Destroy(item);
        }
    }
}