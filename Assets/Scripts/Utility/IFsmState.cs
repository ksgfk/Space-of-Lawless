using UnityEngine;

namespace KSGFK
{
    public interface IFsmState
    {
        int TransitionId { get; set; }
        string Name { get; }

        void OnEnter();

        void OnUpdate();

        void OnLeave();
    }

    public class FsmStateImpl : IFsmState
    {
        public int TransitionId { get; set; } = -1;
        public string Name { get; }

        public FsmStateImpl(string name) { Name = name; }

        public virtual void OnEnter() { }

        public virtual void OnUpdate() { }

        public virtual void OnLeave() { }
    }

    public class FsmStateMonoImpl : MonoBehaviour, IFsmState
    {
        [SerializeField] private int transitionId = -1;
        [SerializeField] private string stateName = null;

        public int TransitionId { get => transitionId; set => value = transitionId; }
        public string Name => stateName;

        public virtual void OnEnter() { }

        public virtual void OnUpdate() { }

        public virtual void OnLeave() { }
    }
}