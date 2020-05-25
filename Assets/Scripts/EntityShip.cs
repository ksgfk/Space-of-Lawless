using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KSGFK
{
    public class EntityShip : Entity
    {
        [SerializeField] private ulong health;
        private LinkedList<ShipModule> _modules = new LinkedList<ShipModule>();

        public ulong Health { get => health; set => health = value; }
        public ICollection<ShipModule> Modules => _modules;

        public void AddModule(ShipModule module)
        {
            module.BaseShip = this;
            _modules.AddLast(module);
            module.transform.SetParent(transform);
            module.OnAddToShip();
        }

        public void RemoveModule(ShipModule module)
        {
            module.OnRemoveFromShip();
            _modules.Remove(module);
            var entry = GameManager.Entity.ShipModuleEntry[module.RuntimeId];
            entry.Destroy(module);
        }

        public override void OnRemoveFromWorld()
        {
            base.OnRemoveFromWorld();
            foreach (var module in Modules.ToArray())
            {
                RemoveModule(module);
            }

            _modules = null;
        }
    }
}