using System;
using UnityEngine;

namespace KSGFK
{
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

        public virtual void OnUse(EntityLiving user) { }

        public virtual int AddItem(int count)
        {
            var isOver = Helper.TryAddValue(nowStack, count, maxStack, out var result, out var overflow);
            nowStack = result;
            if (isOver)
            {
                Debug.Log($"装不下啦，已经溢出来{overflow}啦~~~");
                //TODO:扔出超过部分
            }

            return overflow;
        }
    }
}