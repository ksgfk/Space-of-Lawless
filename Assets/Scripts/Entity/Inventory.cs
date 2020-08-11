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
    public class Inventory : MonoBehaviour
    {
        public Transform holdItemParent;

        private EntityLiving _entity;
        [SerializeField] private int _capacity = 3;
        [SerializeField] private LayerMask _pickUpLayer = 10;
        [SerializeField] private int _usingItemSlot = 0;
        private List<Item> _container = null;
        private CircleCollider2D _collider;
        private List<Collider2D> _overlapCache;

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

        public Item UsingItem => _container[_usingItemSlot];

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
            item.TransferOwner(transform);
            if (index == _usingItemSlot)
            {
                SelectUsingItem(index);
            }
            else
            {
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
                UsingItem.Sprite.enabled = false;
                UsingItem.transform.SetParent(transform, false);
            }

            _usingItemSlot = slot;
            var nextUsing = UsingItem;
            if (nextUsing)
            {
                nextUsing.Sprite.enabled = true;
                var trans = UsingItem.transform;
                trans.localPosition = Vector3.zero;
                trans.SetParent(holdItemParent, false);
            }
        }
    }
}