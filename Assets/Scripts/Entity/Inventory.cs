using System;
using UnityEngine;

namespace KSGFK
{
    [RequireComponent(typeof(CircleCollider2D))]
    [DisallowMultipleComponent]
    public class Inventory : MonoBehaviour
    {
        private EntityLiving _entity;
        [SerializeField] private int _capacity = 3;
        private Item[] _items = null;
        private CircleCollider2D _collider;

        public int Capacity => _capacity;
        public Item[] Container => _items;

        /// <summary>
        /// 拾取半径
        /// </summary>
        public float PickupRadius => _collider.radius;

        /// <summary>
        /// 背包持有者
        /// </summary>
        public EntityLiving Holder => _entity;

        /// <summary>
        /// 是否可以检查拾取半径
        /// </summary>
        public bool CanCheckPickupRadius => _collider.enabled;

        /// <summary>
        /// 可以检查拾取半径时，有物品在范围内触发事件
        /// </summary>
        public event Action<Inventory, Item> TriggerItem;

        public void Init(EntityLiving entity)
        {
            _entity = entity;
            _items = new Item[Capacity];
            _collider = GetComponent<CircleCollider2D>();
            _collider.isTrigger = true;
        }

        public void StartCheckRadius() { _collider.enabled = true; }

        public void StopCheckRadius() { _collider.enabled = false; }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Item"))
            {
                var item = other.GetComponent<Item>();
                TriggerItem?.Invoke(this, item);
            }
        }
    }
}