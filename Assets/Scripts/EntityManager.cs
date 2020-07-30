using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace KSGFK
{
    [Obsolete]
    public class EntityManager : MonoBehaviour
    {
        private GameManager _gm;
        private LinkedList<Entity> _active;

        public ICollection<Entity> ActiveEntity => _active;

        public void Init(GameManager gm)
        {
            _gm = gm;
            _active = new LinkedList<Entity>();
        }

        public Entity SpawnEntity(int id)
        {
            var entry = GetEntry()[id];
            return SpawnEntity(entry);
        }

        public T SpawnEntity<T>(int id) where T : Entity { return SpawnEntity(id) as T; }

        public Entity SpawnEntity(string registerName)
        {
            var entry = GetEntry()[registerName];
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

            var entry = GetEntry()[entity.RuntimeId];
            _active.Remove(entity.Node);
            entry.Destroy(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IRegistry<EntryEntity> GetEntry() { return _gm.Register.Entity; }
    }
}