using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KSGFK
{
    public class AiAgent : MonoBehaviour
    {
        public GameObject around;
        public Vector2 size;

        private FsmSystem _fsm;
        private IFsmState _normal;
        [SerializeField] private Patrol _patrol;

        private void Awake()
        {
            var e = GetComponentInChildren<ShipModuleEngineMono>();
            _normal = new FsmStateImpl("normal");
            _patrol = new Patrol(e, around, size);
            _fsm = new FsmSystem(_normal);
            _fsm.AddState(_patrol);
            _fsm.AddTransition(_normal, _patrol, new NormalToPatrol());
        }

        private void Update() { _fsm.CheckTransition(); }

        [Serializable]
        private class Patrol : FsmStateImpl
        {
            private ShipModuleEngine _engine;
            [SerializeField] private int _state;
            [SerializeField] private Vector2 _target;
            private GameObject _center;
            private Vector2 _size;
            private float _angle;
            private float _time;
            private Vector2 _dir;

            public Patrol(ShipModuleEngine engine, GameObject center, Vector2 size) : base("patrol")
            {
                _engine = engine;
                _state = 0;
                _center = center;
                _size = size;
            }

            public override void OnUpdate()
            {
                switch (_state)
                {
                    case 0:
                        var cen = _center.transform.position;
                        var a = (Vector2) cen - _size;
                        var b = (Vector2) cen + _size;
                        _target = new Vector2(Random.Range(a.x, b.x), Random.Range(a.y, b.y));
                        _state = 1;
                        var rot = _engine.BaseShip.transform.rotation;
                        _angle = Quaternion.Angle(rot, MathExt.FromToRotation(Vector3.up, _target));
                        _time = Mathf.Abs(_angle) / _engine.MaxRotateSpeed + Time.time;
                        break;
                    case 1:
                        _engine.Rotate(_angle);
                        if (Time.time >= _time)
                        {
                            _dir = _target - (Vector2) _engine.BaseShip.transform.position;
                            _time = Time.time + _dir.magnitude / _engine.MaxMoveSpeed;
                            _state = 2;
                        }

                        break;
                    case 2:
                        _engine.MoveDirection(_dir);
                        if (Time.time >= _time)
                        {
                            _time = Time.time + Random.Range(0, 2);
                            _state = 3;
                        }

                        break;
                    case 3:
                        if (Time.time >= _time)
                        {
                            _state = 0;
                        }

                        break;
                }
            }
        }

        private class NormalToPatrol : FsmTransition
        {
            public override bool Check(IFsmState nowState) { return true; }
        }
    }
}