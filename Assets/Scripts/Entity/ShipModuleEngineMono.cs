using Unity.Mathematics;
using UnityEngine;

namespace KSGFK
{
    public class ShipModuleEngineMono : ShipModuleEngine
    {
        public override void MoveDirection(Vector2 direction)
        {
            // BaseShip.transform.Translate(direction.normalized * (Time.deltaTime * MaxMoveSpeed));
            BaseShip.GetComponent<CharacterController2D>()
                .Move(MathExt.TransformDirection(transform.rotation, direction) * (Time.deltaTime * MaxMoveSpeed));
        }

        public override void RotateDelta(Vector2 delta)
        {
            BaseShip.transform.rotation = Quaternion.Slerp(BaseShip.transform.rotation,
                MathExt.FromToRotation(Vector3.up, delta),
                Time.deltaTime * MaxRotateSpeed);
        }

        public override void Rotate(float angle)
        {
            var t = BaseShip.transform;
            var eulerAngles = t.eulerAngles;
            var a = Mathf.Lerp(eulerAngles.z, angle + eulerAngles.z, Time.deltaTime * MaxRotateSpeed);
            t.rotation = Quaternion.AngleAxis(a, Vector3.forward);
        }
    }
}