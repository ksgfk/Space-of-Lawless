namespace KSGFK
{
    public class FsmState
    {
        public int TransitionId { get; set; } = -1;
        public string Name { get; }

        public FsmState(string name) { Name = name; }
        
        public virtual void OnEnter() { }

        public virtual void OnUpdate() { }

        public virtual void OnLeave() { }
    }
}