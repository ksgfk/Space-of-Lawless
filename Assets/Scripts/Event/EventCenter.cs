using System;
using System.Collections.Generic;

namespace KSGFK
{
    public class EventCenter
    {
        private readonly Dictionary<Type, DelegateList> _events;

        public EventCenter() { _events = new Dictionary<Type, DelegateList>(); }

        public void Subscribe<TEvent>(EventHandler<TEvent> e) where TEvent : EventArgs
        {
            var delegateType = e.GetType();
            if (_events.TryGetValue(delegateType, out var list))
            {
                list.Add(e);
            }
            else
            {
                _events.Add(delegateType, new DelegateList(delegateType) {e});
            }
        }

        public void Post<TEvent>(object sender, TEvent e) where TEvent : EventArgs
        {
            var delegateType = typeof(EventHandler<TEvent>);
            if (_events.TryGetValue(delegateType, out var list))
            {
                list.Invoke(sender, e);
            }
        }

        public bool Unsubscribe<TEvent>(EventHandler<TEvent> e) where TEvent : EventArgs
        {
            var delegateType = e.GetType();
            return _events.TryGetValue(delegateType, out var list) && list.Remove(e);
        }

        public void Unsubscribe(Type eventType) { _events.Remove(typeof(EventHandler<>).MakeGenericType(eventType)); }
    }
}