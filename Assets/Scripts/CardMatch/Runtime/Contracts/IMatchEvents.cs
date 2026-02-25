using System;

namespace CardMatch.CardMatch
{
    public interface IMatchEvents
    {
        void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : MatchEvent;
        void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : MatchEvent;
        void Publish<TEvent>(TEvent eventData) where TEvent : MatchEvent;
    }
}
