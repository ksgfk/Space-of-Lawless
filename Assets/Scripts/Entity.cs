using System;
using System.Collections.Generic;

namespace KSGFK
{
    public abstract class Entity : IdentityObject
    {
        private LinkedListNode<Entity> _node;

        internal LinkedListNode<Entity> Node
        {
            get => _node;
            set
            {
                if (_node == null)
                {
                    _node = value;
                }
                else
                {
                    if (value == null)
                    {
                        _node = null;
                    }
                    else
                    {
                        throw new InvalidOperationException("不可重复赋值");
                    }
                }
            }
        }

        public virtual void OnSpawn() { }

        public virtual void OnRemoveFromWorld() { }
    }
}