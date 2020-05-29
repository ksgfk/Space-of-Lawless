using System;
using UnityEngine;

namespace KSGFK
{
    public class AiController : MonoBehaviour
    {
        private FsmMachine _fsm;
        private FsmState _normal;

        private void Awake()
        {
            _normal = new FsmState("normal");
            _fsm = new FsmMachine(_normal);
        }
    }
}