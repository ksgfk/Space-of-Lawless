using DG.Tweening;
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
            _patrol = new Patrol(around, gameObject, size);
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
            private int _state;
            private Vector2 _targetPos;
            private float _rotSpeed = 30;
            private float _movSpeed = 3;
            private Quaternion _targetRot;
            private Tweener _rotTweener;
            private Tweener _movTweener;
            private Vector2 _movDir;

            public Patrol(GameObject center, GameObject go, Vector2 size) : base("patrol")
            {
                _center = center;
                _go = go;
                _size = size;
                _state = 0;
            }

            public override void OnUpdate()
            {
                switch (_state)
                {
                    case 0:
                        var centerPos = (Vector2) _center.transform.position;
                        var less = centerPos - _size;
                        var large = centerPos + _size;
                        _targetPos = new Vector2(Random.Range(less.x, large.x), Random.Range(less.y, large.y));
                        var pos = (Vector2) _go.transform.position;
                        _movDir = _targetPos - pos;
                        var angle = Vector2.SignedAngle(Vector2.up, _movDir);
                        _targetRot = Quaternion.Euler(0, 0, angle);
                        _rotTweener = _go.transform
                            .DORotateQuaternion(_targetRot, Mathf.Abs(angle) / _rotSpeed)
                            .SetAutoKill(false);
                        _state = 1;
                        break;
                    case 1:
                        if (_rotTweener.IsComplete())
                        {
                            _rotTweener.Kill();
                            _movTweener = _go.transform
                                .DOMove(_targetPos, _movDir.magnitude / _movSpeed)
                                .SetAutoKill(false);
                            _state = 2;
                        }

                        break;
                    case 2:
                        if (_movTweener.IsComplete())
                        {
                            _movTweener.Kill();
                            _state = 0;
                            _rotTweener = null;
                            _movTweener = null;
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