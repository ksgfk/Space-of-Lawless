using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] private int runtimeId = -1;
        private LinkedListNode<Entity> _node;

        public int RuntimeId
        {
            get => runtimeId;
            set
            {
                if (runtimeId != -1)
                {
                    throw new InvalidOperationException();
                }

                runtimeId = value;
            }
        }

        internal LinkedListNode<Entity> Node
        {
            get => _node;
            set
            {
                if (_node != null)
                {
                    throw new InvalidOperationException();
                }

                _node = value;
            }
        }
    }
}