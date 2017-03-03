﻿#region File Header

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
using EventBrokerExtension;
using Microsoft.Practices.Unity;

#endregion

namespace SimpleEventBroker
{
    /// <summary>   An event broker. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public class EventBroker
    {
        private IUnityContainer Container { get; }

        /// <summary>   The event publishers. </summary>
        private readonly Dictionary<string, PublishedEvent> eventPublishers = new Dictionary<string, PublishedEvent>();

        public EventBroker(IUnityContainer container)
        {
            Container = container;
        }

        /// <summary>   Gets the registered events. </summary>
        /// <value> The registered events. </value>
        public IEnumerable<string> RegisteredEvents
        {
            get
            {
                foreach(var eventName in eventPublishers.Keys)
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

        public void RegisterWakeupSubscriber(Type declaringType, SubscriptionInfo sub)
        {
            var publishedEvent = GetEvent(sub.PublishedEventName);
            publishedEvent.AddSubscriber(declaringType, sub);
        }

        /// <summary>   Unregisters the subscriber. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publishedEventName">   Name of the published event. </param>
        /// <param name="subscriber">           The subscriber. </param>
        public void UnregisterSubscriber(string publishedEventName, EventHandler<EventArgs> subscriber)
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

        /// <summary>   Gets the subscribers fors in this collection. </summary>
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
            if(!eventPublishers.ContainsKey(eventName))
                eventPublishers[eventName] = new PublishedEvent(Container, this);
            return eventPublishers[eventName];
        }

        /// <summary>   Removes the dead events. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        private void RemoveDeadEvents()
        {
            var deadEvents = new List<string>();
            foreach(var publishedEvent in eventPublishers)
            {
                if(!publishedEvent.Value.HasPublishers && !publishedEvent.Value.HasSubscribers)
                    deadEvents.Add(publishedEvent.Key);
            }

            deadEvents.ForEach(delegate(string eventName) { eventPublishers.Remove(eventName); });
        }
    }
}