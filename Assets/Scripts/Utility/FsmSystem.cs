using System;
using System.Collections.Generic;

namespace KSGFK
{
    public class FsmSystem
    {
        private readonly Dictionary<string, IFsmState> _states;
        private readonly List<FsmTransitionChain> _transitions;

        public IFsmState NowState { get; private set; }
        public IReadOnlyList<FsmTransitionChain> TransitionChains => _transitions;

        public FsmSystem(IFsmState defaultState)
        {
            _states = new Dictionary<string, IFsmState>();
            _transitions = new List<FsmTransitionChain>();
            NowState = defaultState;
            AddState(defaultState);
        }

        public void AddState(IFsmState state)
        {
            if (_states.ContainsKey(state.Name))
            {
                throw new ArgumentException($"已存在状态:{state.Name}");
            }

            _states.Add(state.Name, state);
        }

        public void AddTransition(IFsmState last, IFsmState next, FsmTransition transition)
        {
            if (!_states.ContainsValue(last) || !_states.ContainsValue(next))
            {
                throw new InvalidOperationException("未添加的状态");
            }

            var index = _transitions.FindIndex(i => i.Last == last);
            FsmTransitionChain trans;
            if (index == -1)
            {
                trans = new FsmTransitionChain(last);
                last.TransitionId = _transitions.Count;
                _transitions.Add(trans);
            }
            else
            {
                trans = _transitions[index];
            }

            trans.TransitionList.Add(new FsmTransitionInfo(next, transition));
        }

        public void CheckTransition()
        {
            NowState.OnUpdate();
            var id = NowState.TransitionId;
            if (id < 0)
            {
                return;
            }

            var chain = _transitions[id];
            foreach (var info in chain.transitionList)
            {
                if (info.Transition.Check(NowState))
                {
                    SwitchState(info.Next);
                    break;
                }
            }
        }

        private void SwitchState(IFsmState next)
        {
            NowState.OnLeave();
            next.OnEnter();
            NowState = next;
        }
    }
}