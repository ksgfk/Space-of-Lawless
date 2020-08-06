using System;
using UnityEngine;

namespace KSGFK
{
    [RequireComponent(typeof(Collider2D))]
    [DisallowMultipleComponent]
    public class Item : Entity
    {
        [SerializeField] protected int maxStack;
        [SerializeField] protected int nowStack;

        public int MaxStack
        {
            get
            {
                if (maxStack < 0)
                {
                    throw new ArgumentException($"非法最大堆叠数量:{maxStack}");
                }

                return maxStack;
            }
        }

        public int NowStack
        {
            get
            {
                if (nowStack < 0)
                {
                    throw new ArgumentException($"非法堆叠数量:{nowStack}");
                }

                return nowStack;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException($"非法堆叠数量:{value}");
                }

                if (value > MaxStack)
                {
                    value = MaxStack;
                }

                nowStack = value;
            }
        }

        public override void OnSpawn() { GetComponent<Collider2D>().isTrigger = true; }

        public virtual void OnUse(EntityLiving user) { }

        public virtual int AddItem(int count)
        {
            var isOver = Helper.TryAddValue(nowStack, count, maxStack, out var result, out var overflow);
            nowStack = result;
            return overflow;
        }
    }
}