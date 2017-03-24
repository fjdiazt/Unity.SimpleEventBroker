#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: IEventBrokerInfoPolicy.cs
// Created	: 2014 05 14 12:53
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;

#endregion

namespace EventBrokerExtension
{
    /// <summary>
    ///     This policy interface allows access to saved publication and subscription
    ///     information.
    /// </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public interface IEventBrokerInfoPolicy : IBuilderPolicy
    {
        /// <summary>   Gets the publications. </summary>
        /// <value> The publications. </value>
        IEnumerable<PublicationInfo> Publications { get; }

        /// <summary>   Gets the subscriptions. </summary>
        /// <value> The subscriptions. </value>
        IEnumerable<SubscriptionInfo> Subscriptions { get; }
    }

    /// <summary>   Information about the publication. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public struct PublicationInfo
    {
        /// <summary>   Name of the event. </summary>
        public string EventName;

        /// <summary>   Name of the published event. </summary>
        public string PublishedEventName;

        /// <summary>   Constructor. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publishedEventName">   Name of the published event. </param>
        /// <param name="eventName">            Name of the event. </param>
        public PublicationInfo(string publishedEventName, string eventName)
        {
            PublishedEventName = publishedEventName;
            EventName = eventName;
        }
    }

    /// <summary>   Information about the subscription. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public struct SubscriptionInfo
    {
        /// <summary>   Name of the published event. </summary>
        public string PublishedEventName;

        /// <summary>   The subscriber. </summary>
        public MethodInfo Subscriber;

        public Type EventArgsType { get; }

        public bool Awakable { get; }

        public bool IsAwake { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="publishedEventName">Name of the published event.</param>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="awakable">if set to <c>true</c> [can wake up].</param>
        /// <param name="isAwake">if set to <c>true</c> [is awake].</param>
        /// <exception cref="System.Exception">
        /// Subscriber method must have an EventArgs parameter as the second argument
        /// </exception>
        /// <remarks>
        /// Sander.struijk, 14.05.2014.
        /// </remarks>
        public SubscriptionInfo(string publishedEventName, MethodInfo subscriber, bool awakable = false, bool isAwake = false)
        {
            PublishedEventName = publishedEventName;
            Subscriber = subscriber;
            Awakable = awakable;
            IsAwake = isAwake;

            try
            {
                EventArgsType = subscriber.GetParameters()[ 1 ].ParameterType;
                if (EventArgsType.BaseType != typeof(EventArgs))
                    throw new Exception();
            }
            catch (Exception)
            {
                throw new Exception("Subscriber method must have an EventArgs parameter as the second argument");
            }
        }
    }
}