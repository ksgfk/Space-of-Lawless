using UnityEngine;

namespace KSGFK
{
    public class ShipEngine : MonoBehaviour, IShipModule
    {
        public EntityShip frame;
        public float maxSpeed;

        public GameObject BaseGameObject => gameObject;
        public EntityShip Frame { get => frame; set => frame = value; }
        public MoveData CopyMoveData => new MoveData {Speed = maxSpeed};
    }
}