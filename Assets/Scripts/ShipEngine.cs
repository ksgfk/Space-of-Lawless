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
                NowPos = Frame.transform.position,
                Target = new float3(0, 10, 0),
                Speed = maxRotateSpeed,
                Rotation = Frame.transform.rotation
            };
    }
}