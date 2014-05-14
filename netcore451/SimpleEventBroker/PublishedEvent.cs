#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: PublishedEvent.cs
// Created	: 2014 05 14 13:09
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using System;
using System.Collections.Generic;
using System.Reflection;
using Cocktail;

#endregion

namespace SimpleEventBroker
{
    /// <summary>   A published event. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public class PublishedEvent
    {
        /// <summary>   The publishers. </summary>
        private readonly List<object> publishers;

        /// <summary>   The subscribers. </summary>
        private readonly List<EventHandler> subscribers;

        /// <summary>   Default constructor. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        public PublishedEvent()
        {
            publishers = new List<object>();
            subscribers = new List<EventHandler>();
        }

        /// <summary>   Gets the publishers. </summary>
        /// <value> The publishers. </value>
        public IEnumerable<object> Publishers
        {
            get
            {
                foreach(var publisher in publishers)
                {
                    yield return publisher;
                }
            }
        }

        /// <summary>   Gets the subscribers. </summary>
        /// <value> The subscribers. </value>
        public IEnumerable<EventHandler> Subscribers
        {
            get
            {
                foreach(var subscriber in subscribers)
                {
                    yield return subscriber;
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this SimpleEventBroker.PublishedEvent has
        ///     publishers.
        /// </summary>
        /// <value> true if this SimpleEventBroker.PublishedEvent has publishers, false if not. </value>
        public bool HasPublishers
        {
            get { return publishers.Count > 0; }
        }

        /// <summary>
        ///     Gets a value indicating whether this SimpleEventBroker.PublishedEvent has
        ///     subscribers.
        /// </summary>
        /// <value> true if this SimpleEventBroker.PublishedEvent has subscribers, false if not. </value>
        public bool HasSubscribers
        {
            get { return subscribers.Count > 0; }
        }

        /// <summary>   Adds a publisher to 'eventName'. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publisher">    The publisher. </param>
        /// <param name="eventName">    Name of the event. </param>
        public void AddPublisher(object publisher, string eventName)
        {
            publishers.Add(publisher);
            EventInfo targetEvent = publisher.GetType().GetEvent(eventName);
            GuardEventExists(eventName, publisher, targetEvent);

            MethodInfo addEventMethod = targetEvent.GetAddMethod();
            GuardAddMethodExists(targetEvent);

            EventHandler newSubscriber = OnPublisherFiring;
            addEventMethod.Invoke(publisher, new object[] {newSubscriber});
        }

        /// <summary>   Removes the publisher. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publisher">    The publisher. </param>
        /// <param name="eventName">    Name of the event. </param>
        public void RemovePublisher(object publisher, string eventName)
        {
            publishers.Remove(publisher);
            EventInfo targetEvent = publisher.GetType().GetEvent(eventName);
            GuardEventExists(eventName, publisher, targetEvent);

            MethodInfo removeEventMethod = targetEvent.GetRemoveMethod();
            GuardRemoveMethodExists(targetEvent);

            EventHandler subscriber = OnPublisherFiring;
            removeEventMethod.Invoke(publisher, new object[] {subscriber});
        }

        /// <summary>   Adds a subscriber. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="subscriber">   The subscriber. </param>
        public void AddSubscriber(EventHandler subscriber)
        {
            subscribers.Add(subscriber);
        }

        /// <summary>   Removes the subscriber described by subscriber. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="subscriber">   The subscriber. </param>
        public void RemoveSubscriber(EventHandler subscriber)
        {
            subscribers.Remove(subscriber);
        }

        /// <summary>   Raises the publisher firing event. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        private void OnPublisherFiring(object sender, EventArgs e)
        {
            foreach(var subscriber in subscribers)
            {
                subscriber(sender, e);
            }
        }

        /// <summary>   Queries if a given guard event exists. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="ArgumentException">
        ///     Thrown when one or more arguments have unsupported or
        ///     illegal values.
        /// </exception>
        /// <param name="eventName">    Name of the event. </param>
        /// <param name="publisher">    The publisher. </param>
        /// <param name="targetEvent">  Target event. </param>
        private static void GuardEventExists(string eventName, object publisher, EventInfo targetEvent)
        {
            if(targetEvent == null)
                throw new ArgumentException(string.Format("The event '{0}' is not implemented on type '{1}'",
                        eventName,
                        publisher.GetType().Name));
        }

        /// <summary>   Queries if a given guard add method exists. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="ArgumentException">
        ///     Thrown when one or more arguments have unsupported or
        ///     illegal values.
        /// </exception>
        /// <param name="targetEvent">  Target event. </param>
        private static void GuardAddMethodExists(EventInfo targetEvent)
        {
            if(targetEvent.GetAddMethod() == null)
                throw new ArgumentException(string.Format("The event '{0}' does not have a public Add method",
                        targetEvent.Name));
        }

        /// <summary>   Queries if a given guard remove method exists. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="ArgumentException">
        ///     Thrown when one or more arguments have unsupported or
        ///     illegal values.
        /// </exception>
        /// <param name="targetEvent">  Target event. </param>
        private static void GuardRemoveMethodExists(EventInfo targetEvent)
        {
            if(targetEvent.GetRemoveMethod() == null)
                throw new ArgumentException(string.Format("The event '{0}' does not have a public Remove method",
                        targetEvent.Name));
        }
    }
}