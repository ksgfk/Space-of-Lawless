using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public class EntityManager : MonoBehaviour
    {
        private StageRegistry<EntryEntity> _entity;
        private LinkedList<Entity> _active;

        public IRegistry<EntryEntity> EntityEntry => _entity;
        public ICollection<Entity> ActiveEntity => _active;

        public event Action<IRegistry<EntryEntity>> Register;
        public event Action PostRegister;

        public void Init()
        {
            _entity = new StageRegistry<EntryEntity>("entity");
            _active = new LinkedList<Entity>();
            GameManager.Instance.Init += OnGameInit;
            GameManager.Instance.PostInit += OnGamePostInit;
        }

        private void OnGameInit()
        {
            foreach (var info in GameManager.MetaData.EntityInfo)
            {
                var it = GameManager.TempData.Query<EntryEntity>(GameManager.GetDataPath(info.Path));
                try
                {
                    foreach (var entity in it)
                    {
                        _entity.AddToWaitRegister(entity);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            Register?.Invoke(_entity);
        }

        private void OnGamePostInit()
        {
            _entity.RegisterAll();
            PostRegister?.Invoke();
            Register = null;
            PostRegister = null;
        }

        public Entity SpawnEntity(int id)
        {
            var entry = _entity[id];
            return SpawnEntity(entry);
        }

        public T SpawnEntity<T>(int id) where T : Entity { return SpawnEntity(id) as T; }

        public Entity SpawnEntity(string registerName)
        {
            var entry = _entity[registerName];
            return SpawnEntity(entry);
        }

        public T SpawnEntity<T>(string id) where T : Entity { return SpawnEntity(id) as T; }

        private Entity SpawnEntity(EntryEntity entry)
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

            _active.AddLast(node);
            return instance;
        }

        public void DestroyEntity(Entity entity)
        {
            if (!entity || entity.Node.List == null)
            {
                Debug.LogWarningFormat("已经被销毁的实体:{0}", entity.RuntimeId);
                return;
            }

            var entry = _entity[entity.RuntimeId];
            _active.Remove(entity.Node);
            entry.Destroy(entity);
        }
    }
}