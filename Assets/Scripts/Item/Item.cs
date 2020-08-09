using System;
using UnityEngine;

namespace KSGFK
{
    [DisallowMultipleComponent]
    public class Item : IdentityObject
    {
        [SerializeField] protected int maxStack;
        [SerializeField] protected int nowStack;
        private Collider2D _coll;

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

        public virtual void OnCreate() { }

        public virtual void OnDestroyItem() { }

        public virtual void OnUse(EntityLiving user) { }

        /// <summary>
        /// 合并两个物体
        /// </summary>
        /// <param name="item">将要合并的物品</param>
        /// <returns>合并后，剩余传入物品，若无剩余物品则返回null</returns>
        public Nullable<Item> Merge(Item item)
        {
            // if (!item) throw new ArgumentNullException(nameof(item));
            // Item ret = null;
            // if (IsSameType(this, item))
            // {
            //     World world = GameManager.Instance.World;
            //     if (Helper.TryAddValue(NowStack, item.NowStack, MaxStack, out var result, out var over))
            //     {
            //         var overflowItem = world.CreateItem(item.RuntimeId);
            //         overflowItem.NowStack = over;
            //         ret = overflowItem;
            //     }
            //     else
            //     {
            //         NowStack = result;
            //     }
            //
            //     world.DestroyItem(item);
            // }
            // else
            // {
            //     ret = item;
            // }
            //
            // return new Nullable<Item>(ret);

            return new Nullable<Item>(null);
        }

        /// <summary>
        /// 捡起物品,调用该方法时要注意物品是否在世界中，否则会抛异常.
        /// 只有在世界中的物品才能捡
        /// </summary>
        /// <param name="living">捡拾者</param>
        public virtual void PickedUpFromWorld(EntityLiving living)
        {
            // _coll.enabled = false;
            // GameManager.Instance.World.Value.RemoveItemFromWorld(this);
        }

        /// <summary>
        /// 扔出物品,调用该方法时要注意物品应该不在世界中，否则会抛异常.
        /// 只有不在世界中的物品才能抛到世界
        /// </summary>
        public virtual void ThrownOutIntoWorld()
        {
            // _coll.enabled = true;
            // transform.SetParent(null);
            // GameManager.Instance.World.Value.DropItem(this);
        }

        public static bool IsSameType(Item l, Item r) { return l.RuntimeId == r.RuntimeId; }
    }
}