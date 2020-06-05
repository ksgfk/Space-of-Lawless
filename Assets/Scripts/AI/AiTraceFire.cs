using UnityEngine;

namespace KSGFK
{
    public class AiTraceFire : MonoBehaviour
    {
        public GameObject around;

        private FsmSystem _fsm;
        private IFsmState _normal;
        private Patrol _patrol;

        private void Awake()
        {
            _normal = new FsmStateImpl("normal");
            _patrol = new Patrol(around, gameObject, new Vector2(10, 10));
            _fsm = new FsmSystem(_normal);
            _fsm.AddState(_patrol);
            _fsm.AddTransition(_normal, _patrol, new NormalToPatrol());
        }

        private void Update() { _fsm.CheckTransition(); }

        private class Patrol : FsmStateImpl
        {
            private readonly GameObject _center;
            private readonly GameObject _go;
            private readonly Vector2 _size;

            public Patrol(GameObject center, GameObject go, Vector2 size) : base("patrol")
            {
                _center = center;
                _go = go;
                _size = size;
            }

            public override void OnUpdate()
            {
                var centerPos = (Vector2) _center.transform.position;
                var target = new Vector2(Random.Range(centerPos.x - _size.x, centerPos.x + _size.x),
                    Random.Range(centerPos.y - _size.y, centerPos.y + _size.y));
                _go.transform.position = target * Time.deltaTime;
            }
        }

        private class NormalToPatrol : FsmTransition
        {
            public override bool Check(IFsmState nowState) { return true; }
        }
    }
}