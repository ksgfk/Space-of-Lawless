using System.Collections.Generic;

namespace KSGFK
{
    public class EntityShip : Entity
    {
        public ulong health;

        private LinkedList<IShipModule> _modules = new LinkedList<IShipModule>();

        public LinkedList<IShipModule> Modules => _modules;

        public void AddModule(IShipModule module) { _modules.AddLast(module); }
    }
}