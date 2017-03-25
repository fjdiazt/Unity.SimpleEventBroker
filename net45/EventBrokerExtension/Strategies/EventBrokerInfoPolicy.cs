#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: EventBrokerInfoPolicy.cs
// Created	: 2014 05 14 12:53
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Unity.EventBroker.Strategies
{
    /// <summary>   An event broker information policy. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public class EventBrokerInfoPolicy : IEventBrokerInfoPolicy
    {
        /// <summary>   The publications. </summary>
        private readonly List<PublicationInfo> publications = new List<PublicationInfo>();

        /// <summary>   The subscriptions. </summary>
        private readonly List<SubscriptionInfo> subscriptions = new List<SubscriptionInfo>();

        /// <summary>   Gets the publications. </summary>
        /// <value> The publications. </value>
        public IEnumerable<PublicationInfo> Publications
        {
            get { return publications; }
        }

        /// <summary>   Gets the subscriptions. </summary>
        /// <value> The subscriptions. </value>
        public IEnumerable<SubscriptionInfo> Subscriptions
        {
            get { return subscriptions; }
        }

        /// <summary>   Adds a publication to 'eventName'. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publishedEventName">   Name of the published event. </param>
        /// <param name="eventName">            Name of the event. </param>
        public void AddPublication(string publishedEventName, string eventName)
        {
            publications.Add(new PublicationInfo(publishedEventName, eventName));
        }

        /// <summary>
        /// Adds a subscription to 'subscriber'.
        /// </summary>
        /// <param name="publishedEventName">Name of the published event.</param>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="canWakeUp">if set to <c>true</c> [can wake up].</param>
        /// <param name="isAwake">if set to <c>true</c> [is awake].</param>
        /// <remarks>
        /// Sander.struijk, 14.05.2014.
        /// </remarks>
        public SubscriptionInfo AddSubscription(string publishedEventName, MethodInfo subscriber, 
                                                bool canWakeUp = false, bool isAwake = false)
        {
            var sub = new SubscriptionInfo(publishedEventName, subscriber, canWakeUp, isAwake);
            subscriptions.Add(sub);
            return sub;
        }
    }
}