using UnityEngine;

namespace KSGFK
{
    public class AiTraceFire : MonoBehaviour
    {
        public GameObject around;
        public Vector2 size;

        private FsmSystem _fsm;
        private IFsmState _normal;
        private Patrol _patrol;

        private void Awake()
        {
            _normal = new FsmStateImpl("normal");
            _patrol = new Patrol(GetComponentInChildren<ShipModuleEngine>());
            _fsm = new FsmSystem(_normal);
            _fsm.AddState(_patrol);
            _fsm.AddTransition(_normal, _patrol, new NormalToPatrol());
        }

        private void Update() { _fsm.CheckTransition(); }

        private class Patrol : FsmStateImpl
        {
            private readonly ShipModuleEngine _engine;

            public Patrol(ShipModuleEngine engine) : base("patrol") { _engine = engine; }

            public override void OnUpdate()
            {
                
            }
        }

        private class NormalToPatrol : FsmTransition
        {
            public override bool Check(IFsmState nowState) { return true; }
        }
    }
}