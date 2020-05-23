using System.Collections.Generic;

namespace KSGFK
{
    public ref struct ActionBuffer
    {
        private LinkedList<int> _ints;
        private LinkedList<string> _strs;
        private LinkedList<Entity> _entities;

        public void SpawnEntity(int id) { AddHelper(ref _ints, id); }

        public void SpawnEntity(string name) { AddHelper(ref _strs, name); }

        public void DestroyEntity(Entity entity) { AddHelper(ref _entities, entity); }

        public void Action()
        {
            if (_ints != null)
            {
                foreach (var i in _ints)
                {
                    GameManager.Entity.SpawnEntity(i);
                }
            }

            if (_strs != null)
            {
                foreach (var str in _strs)
                {
                    GameManager.Entity.SpawnEntity(str);
                }
            }

            if (_entities!=null)
            {
                foreach (var entity in _entities)
                {
                    GameManager.Entity.DestroyEntity(entity);
                }
            }
        }

        private static void AddHelper<T>(ref LinkedList<T> list, T val)
        {
            if (list == null)
            {
                list = new LinkedList<T>();
            }

            list.AddLast(val);
        }
    }
}