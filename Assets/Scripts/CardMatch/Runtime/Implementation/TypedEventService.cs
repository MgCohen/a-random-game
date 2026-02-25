using System;
using System.Collections.Generic;

namespace CardMatch.CardMatch
{
    public class TypedEventService : IMatchEvents
    {
        private readonly Dictionary<Type, Delegate> handlers;

        public TypedEventService()
        {
            handlers = new Dictionary<Type, Delegate>();
        }

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : MatchEvent
        {
            Type eventType = typeof(TEvent);
            if (!handlers.TryGetValue(eventType, out Delegate currentHandler))
            {
                handlers[eventType] = handler;
                return;
            }
            handlers[eventType] = Delegate.Combine(currentHandler, handler);
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : MatchEvent
        {
            Type eventType = typeof(TEvent);
            if (!handlers.TryGetValue(eventType, out Delegate currentHandler))
            {
                return;
            }
            Delegate nextHandler = Delegate.Remove(currentHandler, handler);
            if (nextHandler == null)
            {
                handlers.Remove(eventType);
                return;
            }
            handlers[eventType] = nextHandler;
        }

        public void Publish<TEvent>(TEvent eventData) where TEvent : MatchEvent
        {
            Type eventType = typeof(TEvent);
            if (!handlers.TryGetValue(eventType, out Delegate currentHandler))
            {
                return;
            }
            Action<TEvent> typedHandler = currentHandler as Action<TEvent>;
            typedHandler?.Invoke(eventData);
        }
    }
}
