using System.Collections.Generic;

namespace KSGFK
{
    public abstract class Entity : IdentityObject
    {
        private LinkedListNode<Entity> _node;

        internal LinkedListNode<Entity> Node { get => _node; set => _node = Helper.SingleAssign(value, _node != null); }
    }
}