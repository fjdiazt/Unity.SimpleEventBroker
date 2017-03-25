#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: EventBroker.cs
// Created	: 2014 05 14 12:53
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.EventBroker.Strategies;

#endregion

namespace Unity.EventBroker
{
    /// <summary>   An event broker. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public class EventBroker
    {
        /// <summary>   The event publishers. </summary>
        private readonly Dictionary<string, PublishedEvent> _eventPublishers = new Dictionary<string, PublishedEvent>();

        /// <summary>   Gets the registered events. </summary>
        /// <value> The registered events. </value>
        public IEnumerable<string> RegisteredEvents
        {
            get
            {
                foreach(var eventName in _eventPublishers.Keys)
                {
                    yield return eventName;
                }
            }
        }

        /// <summary>   Registers the publisher. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publishedEventName">   Name of the published event. </param>
        /// <param name="publisher">            The publisher. </param>
        /// <param name="eventName">            Name of the event. </param>
        public void RegisterPublisher<T>(string publishedEventName, object publisher, string eventName)
            where T : EventArgs
        {
            var @event = GetEvent(publishedEventName);
            if(@event.Publishers.Any(p=>p.GetType() == publisher.GetType()) == false)
                @event.AddPublisher<T>(publisher, eventName);
        }

        /// <summary>   Unregisters the publisher. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publishedEventName">   Name of the published event. </param>
        /// <param name="publisher">            The publisher. </param>
        /// <param name="eventName">            Name of the event. </param>
        public void UnregisterPublisher<T>(string publishedEventName, object publisher, string eventName)
            where T : EventArgs
        {
            var @event = GetEvent(publishedEventName);
            @event.RemovePublisher<T>(publisher, eventName);
            RemoveDeadEvents();
        }

        /// <summary>   Registers the subscriber. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publishedEventName">   Name of the published event. </param>
        /// <param name="subscriber">           The subscriber. </param>
        public void RegisterSubscriber<T>(string publishedEventName, EventHandler<T> subscriber)
            where T : EventArgs
        {
            var publishedEvent = GetEvent(publishedEventName);
            publishedEvent.AddSubscriber(subscriber);
        }

        /// <summary>
        /// Registers the wakeup subscriber.
        /// </summary>
        /// <param name="declaringType">Type of the declaring.</param>
        /// <param name="sub">The sub.</param>
        public void RegisterAwakableSubscriber(Type declaringType, SubscriptionInfo sub)
        {
            var publishedEvent = GetEvent(sub.PublishedEventName);
            publishedEvent.AddAwakableSubscriber(declaringType, sub);
        }

        /// <summary>   Unregisters the subscriber. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publishedEventName">   Name of the published event. </param>
        /// <param name="subscriber">           The subscriber. </param>
        public void UnregisterSubscriber<T>(string publishedEventName, EventHandler<T> subscriber)
            where T : EventArgs
        {
            var @event = GetEvent(publishedEventName);
            @event.RemoveSubscriber(subscriber);
            RemoveDeadEvents();
        }

        /// <summary>   Gets the publishers fors in this collection. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publishedEvent">   The published event. </param>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the publishers fors in
        ///     this collection.
        /// </returns>
        public IEnumerable<object> GetPublishersFor(string publishedEvent)
        {
            foreach(var publisher in GetEvent(publishedEvent).Publishers)
            {
                yield return publisher;
            }
        }

        /// <summary>   Gets the subscribers for in this collection. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publishedEvent">   The published event. </param>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the subscribers fors in
        ///     this collection.
        /// </returns>
        public IEnumerable<EventHandler<EventArgs>> GetSubscribersFor(string publishedEvent)
        {
            foreach(var subscriber in GetEvent(publishedEvent).Subscribers)
            {
                yield return (EventHandler<EventArgs>) subscriber;
            }
        }

        /// <summary>   Gets an event. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="eventName">    Name of the event. </param>
        /// <returns>   The event. </returns>
        private PublishedEvent GetEvent(string eventName)
        {
            if(!_eventPublishers.ContainsKey(eventName))
                _eventPublishers[eventName] = new PublishedEvent(this);
            return _eventPublishers[eventName];
        }

        /// <summary>   Removes the dead events. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        private void RemoveDeadEvents()
        {
            var deadEvents = new List<string>();
            foreach(var publishedEvent in _eventPublishers)
            {
                if(!publishedEvent.Value.HasPublishers && !publishedEvent.Value.HasSubscribers)
                    deadEvents.Add(publishedEvent.Key);
            }

            deadEvents.ForEach(delegate(string eventName) { _eventPublishers.Remove(eventName); });
        }

        /// <summary>
        /// Cleans up dead publishers and subscribers.
        /// </summary>
        public void CleanUp()
        {
            UnregisterFlaggedPublishers();
            UnregisterFlaggedSubscribers();
        }

        /// <summary>
        /// Unregisters the publishers implementing the IUnsubscribable event that have been
        /// flagged.
        /// </summary>
        private void UnregisterFlaggedPublishers()
        {
            foreach ( var kvp in _eventPublishers )
            {
                var removedPublishers = new List<object>();

                foreach ( var publisher in kvp.Value.Publishers )
                {
                    var disposable = publisher as IUnsubscribableEvent;
                    if ( disposable != null && disposable.Unsubscribe )
                    {
                        removedPublishers.Add( publisher );
                    }
                }

                foreach ( var publisher in removedPublishers )
                {
                    var @event = GetEvent( kvp.Key );
                    @event.RemoveUnTypedPublisher( publisher, kvp.Key );
                }
            }

            RemoveDeadEvents();
        }

        /// <summary>
        /// Unregisters the subscribers implementing the IUnsubscribable event that have been
        /// flagged.
        /// </summary>
        private void UnregisterFlaggedSubscribers()
        {
            foreach ( var kvp in _eventPublishers )
            {
                var removedSubscribers = new List<object>();

                foreach ( var subscriber in kvp.Value.Subscribers )
                {
                    var disposable = ( (dynamic)subscriber ).Target as IUnsubscribableEvent;
                    if ( disposable != null && disposable.Unsubscribe )
                    {
                        removedSubscribers.Add( subscriber );
                    }
                }

                foreach ( var subscriber in removedSubscribers )
                {
                    var @event = GetEvent( kvp.Key );
                    @event.RemoveSubscriber( subscriber );
                }
            }

            RemoveDeadEvents();
        }
    }
}