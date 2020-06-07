using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public class EntryShipEngine : EntryShipModule
    {
        [SerializeField] private string addr = null;
        [SerializeField] private float max_mov_speed = 0;
        [SerializeField] private float max_rot_speed = 0;
        [SerializeField] private GameObject prefab = null;

        public string Addr => addr;
        public float MaxMoveSpeed => max_mov_speed;
        public float MaxRotateSpeed => max_rot_speed;
        public GameObject Prefab { get => prefab; set => prefab = Helper.SingleAssign(value, prefab && value); }

        protected override ShipModule InstantiateBehavior()
        {
            var go = UnityEngine.Object.Instantiate(Prefab);
            go.name = $"{RegisterName}:{RuntimeId}";
            var engine = go.GetComponent<ShipModuleEngine>();
            engine.MaxMoveSpeed = MaxMoveSpeed;
            engine.MaxRotateSpeed = MaxRotateSpeed;
            return engine;
        }

        public override void PerProcess() { GameManager.Load.Request<GameObject>(Addr, sprite => Prefab = sprite); }

        public override void Process() { }

        public override bool Check(out string info)
        {
            var result = Helper.CheckResource(Prefab, Addr, out var reason);
            if (result)
            {
                if (!Prefab.TryGetComponent<ShipModuleEngine>(out _))
                {
                    reason = $"预制体{addr}不存在{typeof(ShipModuleEngine).FullName}组件,忽略";
                }
            }

            info = reason;
            return result;
        }
    }
}