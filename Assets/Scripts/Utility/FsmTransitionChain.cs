using System.Collections.Generic;

namespace KSGFK
{
    public class FsmTransitionChain
    {
        public readonly IFsmState Last;
        internal readonly List<FsmTransitionInfo> TransitionList;

        public IReadOnlyList<FsmTransitionInfo> transitionList => TransitionList;

        public FsmTransitionChain(IFsmState last)
        {
            Last = last;
            TransitionList = new List<FsmTransitionInfo>();
        }
    }

    public class FsmTransitionInfo
    {
        public readonly IFsmState Next;
        public readonly FsmTransition Transition;

        public FsmTransitionInfo(IFsmState next, FsmTransition transition)
        {
            Next = next;
            Transition = transition;
        }
    }
}