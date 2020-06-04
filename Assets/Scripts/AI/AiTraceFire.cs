using System;
using UnityEngine;

namespace KSGFK
{
    public class AiTraceFire : MonoBehaviour
    {
        private FsmSystem _fsm;
        private FsmState _normal;

        private void Awake()
        {
            _normal = new FsmState("normal");
            _fsm = new FsmSystem(_normal);
        }
    }
}