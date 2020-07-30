using UnityEngine;

namespace KSGFK
{
    [DisallowMultipleComponent]
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private int _capacity = 3;
        [SerializeField] private Item[] _items = null;

        public int Capacity => _capacity;
        public Item[] Container => _items;

        private void Awake() { _items = new Item[Capacity]; }
    }
}