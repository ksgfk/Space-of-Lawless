namespace KSGFK
{
    public abstract class FsmTransition
    {
        public FsmState NextState { get; set; }

        public abstract bool Check(FsmState nowState);
    }
}