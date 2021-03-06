using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KSGFK
{
    /// <summary>
    /// 背包
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    [DisallowMultipleComponent]
    public class Inventory : MonoBehaviour, IEnumerable<Item>
    {
        public Transform holdItemParent;

        private EntityLiving _entity;
        [SerializeField] private int _capacity = 3;
        [SerializeField] private LayerMask _pickUpLayer = 10;
        [SerializeField] private int _usingItemSlot = 0;
        private List<Item> _container = null;
        private CircleCollider2D _collider;
        private List<Collider2D> _overlapCache;
        private HashSet<int> _modify;

        /// <summary>
        /// 最大容量
        /// </summary>
        public int Capacity => _capacity;

        /// <summary>
        /// 拾取半径
        /// </summary>
        public float PickupRadius => _collider.radius;

        /// <summary>
        /// 背包持有者
        /// </summary>
        public EntityLiving Holder => _entity;

        public Item UsingItem { get => this[_usingItemSlot]; set => this[_usingItemSlot] = value; }
        public int UsingSlot => _usingItemSlot;

        public Item this[int index]
        {
            get
            {
                var i = _container[index];
                if (i)
                {
                    _modify.Add(index);
                }

                return i;
            }
            set
            {
                _container[index] = value;
                _modify.Add(index);
            }
        }

        private void LateUpdate()
        {
            if (_modify.Count != 0)
            {
                foreach (var i in _modify)
                {
                    Clear(i);
                }

                _modify.Clear();
            }
        }

        public void Init(EntityLiving entity)
        {
            _entity = entity;
            _container = new List<Item>(_capacity);
            for (var i = 0; i < _capacity; i++)
            {
                _container.Add(null);
            }

            _collider = GetComponent<CircleCollider2D>();
            _collider.isTrigger = true;
            _overlapCache = new List<Collider2D>();
            if (!holdItemParent)
            {
                Debug.LogError("使用中物品的节点不存在");
            }

            _modify = new HashSet<int>();

            _entity.Cc2d.TurnLeft += _ => holdItemParent.RotateMirror(false, false);
            _entity.Cc2d.TurnRight += _ => holdItemParent.RotateMirror(true, true);
        }

        /// <summary>
        /// 检查拾取范围内的物品
        /// </summary>
        public IEnumerable<EntityItem> CheckPickupRadius()
        {
            var filter = new ContactFilter2D().NoFilter();
            filter.SetLayerMask(_pickUpLayer);
            _collider.OverlapCollider(filter, _overlapCache);
            return _overlapCache.Select(cache => cache.GetComponent<EntityItem>());
        }

        /// <summary>
        /// 将物品存入背包
        /// </summary>
        /// <param name="items"></param>
        public void PickupItems(IEnumerable<EntityItem> items)
        {
            foreach (var item in items)
            {
                var result = InsertItem(item.Hold);
                item.Hold = result;
            }
        }

        public Item InsertItem(Item item)
        {
            if (FindSameTypeIndex(item, out var index)) //找背包中相同Item
            {
                var res = _container[index].Merge(item); //找到的话尝试合并
                if (!res) //全部合并进去了，直接返回
                {
                    return null;
                }
            }

            if (!FindNullSlotIndex(out index))
            {
                return item;
            }

            _container[index] = item; //还有空余位置
            if (index == _usingItemSlot)
            {
                SelectUsingItem(index);
            }
            else
            {
                item.transform.SetParent(transform, false);
                item.Sprite.enabled = false;
            }

            return null;
        }

        /// <summary>
        /// 捡起范围内所有物品
        /// </summary>
        public void PickupRadiusItems()
        {
            var eis = CheckPickupRadius();
            PickupItems(eis);
        }

        private bool FindSameTypeIndex(Item item, out int index)
        {
            for (var i = 0; i < _container.Count; i++)
            {
                var contents = _container[i];
                if (!contents)
                {
                    continue;
                }

                if (Item.IsSameType(contents, item))
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        private bool FindNullSlotIndex(out int index)
        {
            for (var i = 0; i < _container.Count; i++)
            {
                var item = _container[i];
                if (!item)
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        public void SelectUsingItem(int slot)
        {
            if (slot < 0 || slot >= Capacity)
            {
                Debug.LogWarning("无效的背包格子");
                return;
            }

            var nowUsing = UsingItem;
            if (nowUsing)
            {
                nowUsing.Sprite.enabled = false;
                MoveToNoUse(nowUsing);
            }

            _usingItemSlot = slot;
            var nextUsing = UsingItem;
            if (nextUsing)
            {
                nextUsing.Sprite.enabled = true;
                MoveToUse(nextUsing);
            }
        }

        private void MoveToUse(Item usingItem)
        {
            var trans = UsingItem.transform;
            trans.SetParent(holdItemParent, false);
            trans.localPosition = usingItem.RotateOffset;
            trans.RotateMirror(true, true);
        }

        private void MoveToNoUse(Item noUse) { noUse.transform.SetParent(transform, false); }

        public void UseHeldItem()
        {
            var item = UsingItem;
            if (item)
            {
                item.OnUse(Holder);
            }
        }

        public void Rotate(Vector2 targetPos)
        {
            var invTrans = holdItemParent;
            Vector2 invPos = invTrans.position;
            //这里不用Job，因为Job是该帧结束时才计算，可能会导致一些问题
            var degree = Vector3.SignedAngle(Vector3.right, targetPos - invPos, Vector3.forward);
            invTrans.rotation = Quaternion.Euler(0, 0, degree);
        }

        public EntityItem DropUsingItem()
        {
            var us = UsingItem;
            if (!us)
            {
                return null;
            }

            World world = GameManager.Instance.World;
            var willDrop = us;
            var ei = world.CreateItemInWorld(willDrop, Holder);
            willDrop.transform.RotateMirror(_entity.Cc2d.Face == FaceDirection.Right, true);
            UsingItem = null;
            ei.transform.position = Holder.transform.position;
            return ei;
        }

        public IEnumerator<Item> GetEnumerator()
        {
            foreach (var item in _container)
            {
                if (item)
                {
                    yield return item;
                }
            }

            for (var i = 0; i < _container.Count; i++)
            {
                Clear(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        private void Clear(int index)
        {
            var item = _container[index];
            if (item && item.IsEmpty)
            {
                GameManager.Instance.World.Value.DestroyItem(item);
                _container[index] = null;
            }
        }

        public int FindFirst(Func<Item, bool> predicate)
        {
            for (var i = 0; i < _container.Count; i++)
            {
                var item = _container[i];
                if (predicate(item))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}