using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGCombat.Domain
{
    public class Bus
    {
        private static ConcurrentDictionary<int, Subscriber> subscribers = new ConcurrentDictionary<int, Subscriber>();

        public static void Subscribe<T>(Action<T> handler)
        {
            var anonymousSubscriber = new AnonymousSubscriber<T>(handler);
            subscribers.TryAdd(anonymousSubscriber.GetHashCode(), anonymousSubscriber);
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            Subscriber anonymousSubscriber = new AnonymousSubscriber<T>(handler);
            subscribers.TryRemove(anonymousSubscriber.GetHashCode(),
                out anonymousSubscriber);
        }

        public static void Raise<T>(T domainEvent)
        {
            foreach (var handlerOfT in subscribers.Values.OfType<Subscriber<T>>())
            {
                try
                {
                    handlerOfT.Handle(domainEvent);
                }
                catch (Exception ex)
                {
                    //log exception here and throw
                    throw;
                }
            }
        }

        public static void Clear()
        {
            subscribers = null;
            subscribers = new ConcurrentDictionary<int, Subscriber>();
        }

        private class AnonymousSubscriber<T> : Subscriber<T>
        {
            private readonly Action<T> handler;

            public AnonymousSubscriber(Action<T> handler)
            {
                this.handler = handler;
            }
            public void Handle(T domainEvent)
            {
                handler(domainEvent);
            }

            public override bool Equals(object obj)
            {
                return handler == ((AnonymousSubscriber<T>)obj).handler;
            }

            public override int GetHashCode()
            {
                return (handler != null ? handler.GetHashCode() : 0);
            }
        }
    }

    /// <summary>
    /// A subscriber will handle any domain events that are raised.
    /// </summary>
    public interface Subscriber { }

    public interface Subscriber<in T> : Subscriber
    {
        void Handle(T domainEvent);
    }
}
