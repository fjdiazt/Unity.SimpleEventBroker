#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: EventBrokerWireupStrategy.cs
// Created	: 2014 05 14 13:09
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using System;
using Microsoft.Practices.ObjectBuilder2;
using SimpleEventBroker;
using Catel.Reflection;

#endregion

namespace EventBrokerExtension
{
    /// <summary>   An event broker wireup strategy. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public class EventBrokerWireupStrategy : BuilderStrategy
    {
        /// <summary>
        ///     Called during the chain of responsibility for a build operation. The PreBuildUp
        ///     method is called when the chain is being executed in the forward direction.
        /// </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="context">  Context of the build operation. </param>
        public override void PreBuildUp(IBuilderContext context)
        {
            if(context.Existing != null)
            {
                var policy = context.Policies.Get<IEventBrokerInfoPolicy>(context.BuildKey);
                if(policy != null)
                {
                    var broker = GetBroker(context);
                    foreach(var pub in policy.Publications)
                    {
                        broker.RegisterPublisher(pub.PublishedEventName, context.Existing, pub.EventName);
                    }
                    foreach(var sub in policy.Subscriptions)
                    {
                        broker.RegisterSubscriber(sub.PublishedEventName, (EventHandler)DelegateHelper.CreateDelegate(typeof(EventHandler), context.Existing, sub.Subscriber));
                    }
                }
            }
        }

        /// <summary>   Gets a broker. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the requested operation is
        ///     invalid.
        /// </exception>
        /// <param name="context">  The context. </param>
        /// <returns>   The broker. </returns>
        private EventBroker GetBroker(IBuilderContext context)
        {
            var broker = context.NewBuildUp<EventBroker>();
            if(broker == null)
                throw new InvalidOperationException("No event broker available");
            return broker;
        }
    }
}