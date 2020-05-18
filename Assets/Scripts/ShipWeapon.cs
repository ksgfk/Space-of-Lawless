using UnityEngine;

namespace KSGFK
{
    public class ShipWeapon : MonoBehaviour, IShipModule
    {
        public GameObject BaseGameObject { get; }

        public EntityShip Frame { get; set; }
    }
}