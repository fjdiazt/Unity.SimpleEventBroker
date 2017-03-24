#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: EventBrokerWireupStrategy.cs
// Created	: 2014 05 14 12:53
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;

#endregion

namespace EventBrokerExtension
{
    /// <summary>   An event broker wireup strategy. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public class EventBrokerWireupStrategy : BuilderStrategy
    {
        private static IEnumerable<MethodInfo> WakeupEventsCache { get; set; }

        private bool BuildingUp { get; set; }

        /// <summary>   Pre build up. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="context">  The context. </param>
        public override void PreBuildUp(IBuilderContext context)
        {
            if(BuildingUp)
                return;

            if (context.BuildKey?.Type?.IsDefined( typeof( PublisherAttribute ) ,false) == false 
                && context.BuildKey?.Type.IsDefined(typeof(SubscriberAttribute), false) == false)
                return;

            if(context.Existing != null )
            {
                var policy = context.Policies.Get<IEventBrokerInfoPolicy>(context.BuildKey);
                if(policy != null)
                {
                    var broker = GetBroker(context);

                    BuildPublications(context, broker, policy);

                    BuildSubscriptions(context, broker, policy);
                }
            }
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PostBuildUp(IBuilderContext context)
        {
            BuildingUp = false;
            base.PostBuildUp(context);
        }

        private void BuildPublications(IBuilderContext context, EventBroker.EventBroker broker, IEventBrokerInfoPolicy policy)
        {
            var registerPublisher = broker.GetType().GetMethod( nameof( broker.RegisterPublisher ) );

            foreach ( var pub in policy.Publications )
            {
                var eventArgsType = GetEventArGetArgsType( context.OriginalBuildKey.Type, pub.EventName );

                registerPublisher.MakeGenericMethod(eventArgsType)
                                 .Invoke(broker, new[] {pub.PublishedEventName, context.Existing, pub.EventName});
            }
        }

        private void BuildSubscriptions(IBuilderContext context, EventBroker.EventBroker broker, 
                                               IEventBrokerInfoPolicy policy)
        {
            var registerSubscriber = broker.GetType().GetMethod( nameof( broker.RegisterSubscriber ) );

            foreach ( var sub in policy.Subscriptions.Where(s=>s.IsAwake) )
            {
                var @delegate = Delegate
                    .CreateDelegate(typeof(EventHandler<>).MakeGenericType(sub.EventArgsType),
                                    context.Existing, sub.Subscriber);

                registerSubscriber.MakeGenericMethod( sub.EventArgsType )
                                  .Invoke( broker, new object[] { sub.PublishedEventName, @delegate } );
            }

            foreach ( var sub in policy.Subscriptions.Where( s => s.Awakable && !s.IsAwake ) )
            {
                if ( sub.Subscriber.DeclaringType == null )
                    throw new Exception( $"Unable to get declaring type for event {sub.Subscriber.Name}" );

                broker.RegisterWakeupSubscriber( sub.Subscriber.DeclaringType, sub );
            }
        }

        private Type GetEventArGetArgsType(Type objectType, string eventName)
        {
            var @event = objectType.GetEvent(eventName);
            if(@event == null)
                throw new Exception($"The type '{objectType}' does not have the event '{eventName}'");

            return @event.EventHandlerType.GenericTypeArguments[0]
                           ?? typeof( EventArgs );
        }

        /// <summary>   Gets a broker. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the requested operation is
        ///     invalid.
        /// </exception>
        /// <param name="context">  The context. </param>
        /// <returns>   The broker. </returns>
        private EventBroker.EventBroker GetBroker(IBuilderContext context)
        {
            var broker = context.NewBuildUp<EventBroker.EventBroker>();
            if(broker == null)
                throw new InvalidOperationException("No event broker available");
            return broker;
        }

        private IEnumerable<MethodInfo> GetWakeupSubscriberMethods()
        {
            if ( WakeupEventsCache != null )
                return WakeupEventsCache;

            WakeupEventsCache = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(GetLoadableTypes)
                .SelectMany(t => t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public |
                                              BindingFlags.Instance)
                                  .Where(m => m.IsDefined(typeof(SubscribesToAttribute))))
                .Where(m => m.GetCustomAttributes<SubscribesToAttribute>()
                             .Any(a => a.Awakable));

            return WakeupEventsCache;
        }

        private static IEnumerable<Type> GetLoadableTypes( Assembly assembly )
        {
            try
            {
                return assembly.GetTypes().Where( t => t.IsPublic && t.IsDefined( typeof( SubscriberAttribute ) ) );
            }
            catch ( ReflectionTypeLoadException e )
            {
                return e.Types.Where( t => t != null );
            }
        }
    }
}