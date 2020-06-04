using System;
using System.Collections.Generic;

namespace KSGFK
{
    public readonly struct TransitionChain
    {
        public readonly FsmState Last;
        internal readonly List<TransitionInfo> nextList;

        public IReadOnlyList<TransitionInfo> NextList => nextList;

        public TransitionChain(FsmState last)
        {
            Last = last;
            nextList = new List<TransitionInfo>();
        }
    }

    public readonly struct TransitionInfo
    {
        public readonly FsmState Next;
        public readonly FsmTransition Transition;

        public TransitionInfo(FsmState next, FsmTransition transition)
        {
            Next = next;
            Transition = transition;
        }
    }

    public class FsmSystem
    {
        private readonly Dictionary<string, FsmState> _states;
        private readonly List<TransitionChain> _transitions;

        public FsmState NowState { get; private set; }
        public IReadOnlyList<TransitionChain> TransitionChains => _transitions;

        public FsmSystem(FsmState defaultState)
        {
            _states = new Dictionary<string, FsmState>();
            _transitions = new List<TransitionChain>();
            NowState = defaultState;
        }

        public void AddState(FsmState state)
        {
            if (_states.ContainsKey(state.Name))
            {
                throw new ArgumentException($"已存在状态:{state.Name}");
            }

            _states.Add(state.Name, state);
        }

        public void AddTransition(FsmState last, FsmState next, FsmTransition transition)
        {
            if (!_states.ContainsValue(last) || !_states.ContainsValue(next))
            {
                throw new InvalidOperationException("未添加的状态");
            }

            var index = _transitions.FindIndex(i => i.Last == last);
            TransitionChain trans;
            if (index == -1)
            {
                trans = new TransitionChain(last);
                last.TransitionId = _transitions.Count;
                _transitions.Add(trans);
            }
            else
            {
                trans = _transitions[index];
            }

            trans.nextList.Add(new TransitionInfo(next, transition));
        }

        public void CheckTransition()
        {
            var id = NowState.TransitionId;
            if (id < 0)
            {
                return;
            }

            var chain = _transitions[id];
            foreach (var info in chain.NextList)
            {
                if (info.Transition.Check(NowState))
                {
                    SwitchState(info.Next);
                    break;
                }
            }
        }

        private void SwitchState(FsmState next)
        {
            NowState.OnLeave();
            next.OnEnter();
            NowState = next;
        }
    }
}