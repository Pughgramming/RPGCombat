using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGCombat.Domain
{
    /// <summary>
    /// Domain event bus. Used to set up domain events and subscribers to those events.
    /// </summary>
    public class Bus
    {
        private static ConcurrentDictionary<int, Subscriber> subscribers = new ConcurrentDictionary<int, Subscriber>();

        /// <summary>
        /// Subscribe to an event of a given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public static void Subscribe<T>(Action<T> handler)
        {
            var anonymousSubscriber = new AnonymousSubscriber<T>(handler);
            // add to subscriber dictionary
            subscribers.TryAdd(anonymousSubscriber.GetHashCode(), anonymousSubscriber);
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            Subscriber anonymousSubscriber = new AnonymousSubscriber<T>(handler);
            //remove subscriber
            subscribers.TryRemove(anonymousSubscriber.GetHashCode(),
                out anonymousSubscriber);
        }

        /// <summary>
        /// Raise event type and call handler for all subscribers.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domainEvent"></param>
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

        /// <summary>
        /// Flush subscribers from dictionary.
        /// </summary>
        public static void Clear()
        {
            subscribers = null;
            subscribers = new ConcurrentDictionary<int, Subscriber>();
        }

        private class AnonymousSubscriber<T> : Subscriber<T>
        {
            private readonly Action<T> handler;

            //add handler
            public AnonymousSubscriber(Action<T> handler)
            {
                this.handler = handler;
            }

            /// <summary>
            /// Handle given event type. (Atack/Heal etc.)
            /// </summary>
            /// <param name="domainEvent"></param>
            public void Handle(T domainEvent)
            {
                handler(domainEvent);
            }

            /// <summary>
            /// Get unique hash code for key.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return (handler != null ? handler.GetHashCode() : 0);
            }
        }
    }


    public interface Subscriber { }

    /// <summary>
    /// And Event Subscriber.
    /// </summary>
    public interface Subscriber<in T> : Subscriber
    {
        void Handle(T domainEvent);
    }
}
