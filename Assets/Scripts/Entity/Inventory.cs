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
        public GameObject holdItemParent;

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

            //没找到相同Item，且没合并完
            if (_container.Count >= Capacity)
            {
                return item; //背包没有更多容量了
            }

            _container.Add(item); //还有空余位置
            item.TransferOwner(transform);
            return null;
        }

        /// <summary>
        /// 捡起范围内所有物品
        /// </summary>
        public void PickRadiusItems()
        {
            var eis = CheckPickupRadius();
            PickupItems(eis);
        }

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
    }
}