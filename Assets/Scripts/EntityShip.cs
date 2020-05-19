using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public class EntityShip : Entity
    {
        [SerializeField] private ulong health;
        private readonly LinkedList<ShipModule> _modules = new LinkedList<ShipModule>();

        public ulong Health { get => health; set => health = value; }
        public ICollection<ShipModule> Modules => _modules;

        public void AddModule(ShipModule module)
        {
            module.BaseShip = this;
            _modules.AddLast(module);
        }
    }
}