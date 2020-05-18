using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public class EntityShip : Entity
    {
        [SerializeField] private ulong health;
        private LinkedList<IShipModule> _modules;

        public ulong Health { get => health; set => health = value; }
        public IReadOnlyCollection<IShipModule> Modules => _modules;

        public void AddModule(IShipModule module)
        {
            if (_modules == null)
            {
                _modules = new LinkedList<IShipModule>();
            }

            _modules.AddLast(module);
        }
    }
}