using Unity.Mathematics;
using UnityEngine;

namespace KSGFK
{
    public class ShipEngine : MonoBehaviour, IShipModule
    {
        public EntityShip frame;
        public float maxMoveSpeed;
        public float maxRotateSpeed;

        public GameObject BaseGameObject => gameObject;
        public EntityShip Frame { get => frame; set => frame = value; }
        public MoveData CopyMoveData => new MoveData {Speed = maxMoveSpeed};

        public RotateData CopyRotateData =>
            new RotateData
            {
                Speed = maxRotateSpeed,
                Rotation = quaternion.identity
            };
    }
}