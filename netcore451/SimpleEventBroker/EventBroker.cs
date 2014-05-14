#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: EventBroker.cs
// Created	: 2014 05 14 13:09
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using System;
using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;

#endregion

namespace SimpleEventBroker
{
    /// <summary>   An event broker. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public class EventBroker
    {
        /// <summary>   The event publishers. </summary>
        private readonly Dictionary<string, PublishedEvent> eventPublishers = new Dictionary<string, PublishedEvent>();

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
        public void RegisterPublisher(string publishedEventName, object publisher, string eventName)
        {
            var @event = GetEvent(publishedEventName);
            @event.AddPublisher(publisher, eventName);
        }

        /// <summary>   Unregisters the publisher. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publishedEventName">   Name of the published event. </param>
        /// <param name="publisher">            The publisher. </param>
        /// <param name="eventName">            Name of the event. </param>
        public void UnregisterPublisher(string publishedEventName, object publisher, string eventName)
        {
            var @event = GetEvent(publishedEventName);
            @event.RemovePublisher(publisher, eventName);
            RemoveDeadEvents();
        }

        /// <summary>   Registers the subscriber. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publishedEventName">   Name of the published event. </param>
        /// <param name="subscriber">           The subscriber. </param>
        public void RegisterSubscriber(string publishedEventName, EventHandler subscriber)
        {
            var publishedEvent = GetEvent(publishedEventName);
            publishedEvent.AddSubscriber(subscriber);
        }

        /// <summary>   Unregisters the subscriber. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publishedEventName">   Name of the published event. </param>
        /// <param name="subscriber">           The subscriber. </param>
        public void UnregisterSubscriber(string publishedEventName, EventHandler subscriber)
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
        public IEnumerable<EventHandler> GetSubscribersFor(string publishedEvent)
        {
            foreach(var subscriber in GetEvent(publishedEvent).Subscribers)
            {
                yield return subscriber;
            }
        }

        /// <summary>   Gets an event. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="eventName">    Name of the event. </param>
        /// <returns>   The event. </returns>
        private PublishedEvent GetEvent(string eventName)
        {
            if(!eventPublishers.ContainsKey(eventName))
                eventPublishers[eventName] = new PublishedEvent();
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