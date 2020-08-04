using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace KSGFK
{
    [DisallowMultipleComponent]
    public class World : MonoBehaviour, IDisposable
    {
        protected GameManager _gm;
        private SceneInstance _sceneInstance;
        private LinkedList<Entity> _activeEntity;
        private PoolCenter _pool;

        public SceneInstance Scene => _sceneInstance;
        public IEnumerable<Entity> ActiveEntity => _activeEntity;
        public PoolCenter Pool => _pool;

        public virtual void Init(GameManager gm, in SceneInstance sceneInstance)
        {
            _gm = gm;
            _sceneInstance = sceneInstance;
            _activeEntity = new LinkedList<Entity>();
            _pool = new PoolCenter();
        }

        protected virtual Entity SpawnEntity(EntryEntity entry)
        {
            if (entry == null)
            {
                return null;
            }

            var instance = entry.Instantiate();
            LinkedListNode<Entity> node;
            if (instance.Node == null)
            {
                node = new LinkedListNode<Entity>(instance);
                instance.Node = node;
            }
            else
            {
                node = instance.Node;
            }

            _activeEntity.AddLast(node);
            return instance;
        }

        public Entity SpawnEntity(int id)
        {
            var entry = GetEntityRegistry()[id];
            return SpawnEntity(entry);
        }

        public T SpawnEntity<T>(int id) where T : Entity { return SpawnEntity(id) as T; }

        public Entity SpawnEntity(string registerName)
        {
            var entry = GetEntityRegistry()[registerName];
            return SpawnEntity(entry);
        }

        public T SpawnEntity<T>(string id) where T : Entity { return SpawnEntity(id) as T; }

        public void DestroyEntity(Entity entity)
        {
            if (!entity || entity.Node.List == null)
            {
                Debug.LogWarningFormat("已经被销毁的实体:{0}", entity.RuntimeId);
                return;
            }

            var entry = GetEntityRegistry()[entity.RuntimeId];
            _activeEntity.Remove(entity.Node);
            entry.Destroy(entity);
        }

        protected virtual IRegistry<EntryEntity> GetEntityRegistry() { return _gm.Register.Entity; }

        public Item CreateItem(string itemId) { return CreateItem(GetItemRegistry()[itemId]); }

        public Item CreateItem(int itemId) { return CreateItem(GetItemRegistry()[itemId]); }

        public Item CreateItem(string itemId, Vector2 pos)
        {
            var item = CreateItem(GetItemRegistry()[itemId]);
            item.transform.position = pos;
            return item;
        }

        public Item CreateItem(int itemId, Vector2 pos)
        {
            var item = CreateItem(GetItemRegistry()[itemId]);
            item.transform.position = pos;
            return item;
        }

        protected virtual Item CreateItem(EntryItem entryItem) { return (Item) entryItem.Instantiate(); }

        protected virtual IRegistry<EntryItem> GetItemRegistry() { return _gm.Register.Item; }

        public void Dispose() { Pool.Dispose(); }
    }
}