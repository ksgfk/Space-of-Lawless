namespace KSGFK
{
    public abstract class FsmTransition
    {
        public IFsmState NextState { get; set; }

        public abstract bool Check(IFsmState nowState);
    }
}