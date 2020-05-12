using System.Collections.Generic;
using UnityEngine;

namespace KSGFK
{
    public abstract class Entity : MonoBehaviour
    {
        public int runtimeId;
        public int generation;
        public LinkedListNode<Entity> node;
    }
}