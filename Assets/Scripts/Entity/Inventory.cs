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
        private EntityLiving _entity;
        [SerializeField] private int _capacity = 3;
        [SerializeField] private LayerMask _pickUpLayer = 10;
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

        public void Init(EntityLiving entity)
        {
            _entity = entity;
            _container = new List<Item>(_capacity);
            _collider = GetComponent<CircleCollider2D>();
            _collider.isTrigger = true;
            _overlapCache = new List<Collider2D>();
        }

        /// <summary>
        /// 检查拾取范围内的物品
        /// </summary>
        public IEnumerable<Item> CheckPickupRadius()
        {
            var filter = new ContactFilter2D().NoFilter();
            filter.SetLayerMask(_pickUpLayer);
            _collider.OverlapCollider(filter, _overlapCache);
            return _overlapCache.Select(cache => cache.GetComponent<Item>());
        }

        /// <summary>
        /// 将物品存入背包
        /// </summary>
        /// <param name="items"></param>
        public void SaveItems(IEnumerable<Item> items)
        {
            foreach (var willSave in items)
            {
                if (FindSameTypeIndex(willSave, out var sameIndex))
                {
                    var mergeOver = _container[sameIndex].Merge(willSave);
                    if (mergeOver.HasValue)
                    {
                        DropOverWhenInsertItem(mergeOver.Value);
                    }
                }
                else
                {
                    DropOverWhenInsertItem(willSave);
                }
            }
        }

        /// <summary>
        /// 捡起范围内所有物品
        /// </summary>
        public void PickRadiusItems() { SaveItems(CheckPickupRadius()); }

        private bool FindSameTypeIndex(Item item, out int index)
        {
            for (var i = 0; i < _container.Count; i++)
            {
                var contents = _container[i];
                if (Item.IsSameType(contents, item))
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        private void DropOverWhenInsertItem(Item item)
        {
            if (_container.Count >= _capacity)
            {
                if (!item.IsInWorld)
                {
                    item.ThrownOutIntoWorld();
                }
            }
            else
            {
                _container.Add(item);
                item.PickedUpFromWorld(_entity);
                item.transform.SetParent(transform);
            }
        }
    }
}