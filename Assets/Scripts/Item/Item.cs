using System;
using UnityEngine;

namespace KSGFK
{
    [RequireComponent(typeof(SpriteRenderer))]
    [DisallowMultipleComponent]
    public class Item : IdentityObject
    {
        [SerializeField] private int _maxStack;
        [SerializeField] private int _nowStack;
        [SerializeField] private Transform _rotateCenter;
        private SpriteRenderer _sprite;

        public int MaxStack => _maxStack;

        public int NowStack
        {
            get
            {
                if (_nowStack <= 0)
                {
                    throw new ArgumentException($"非法堆叠数量:{_nowStack}");
                }

                return _nowStack;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException($"非法堆叠数量:{value}");
                }

                if (value == 0)
                {
                    GameManager.Instance.World.Value.DestroyItem(this);
                }

                if (value > MaxStack)
                {
                    value = MaxStack;
                }

                _nowStack = value;
            }
        }

        public Vector2 RotateOffset => -(Vector2) _rotateCenter.localPosition;

        public SpriteRenderer Sprite => _sprite;

        public virtual void OnCreate()
        {
            _sprite = GetComponent<SpriteRenderer>();
            if (!_rotateCenter)
            {
                _rotateCenter = transform;
            }
        }

        public virtual void OnDestroyItem() { }

        public virtual void OnUse(EntityLiving user) { }

        /// <summary>
        /// 合并两个物体
        /// </summary>
        /// <param name="item">将要合并的物品</param>
        /// <returns>合并后，剩余传入物品，若无剩余物品则返回null</returns>
        public Item Merge(Item item)
        {
            if (!item)
            {
                return null;
            }

            if (!IsSameType(this, item))
            {
                return item;
            }

            var isOver = Helper.TryAddValue(NowStack, item.NowStack, MaxStack, out var result, out var overflow);
            NowStack = result;
            item.NowStack = overflow;
            return isOver ? item : null;
        }

        public static bool IsSameType(Item l, Item r) { return l.RuntimeId == r.RuntimeId; }

        public void SetMaxStack(int maxStack)
        {
            if (maxStack <= 0) throw new ArgumentException();
            _maxStack = maxStack;
        }

        public void TransferOwner(Transform owner) { transform.SetParent(owner); }
    }
}