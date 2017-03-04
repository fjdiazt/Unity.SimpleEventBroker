#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: PublishedEvent.cs
// Created	: 2014 05 14 12:53
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EventBrokerExtension;
using EventBrokerExtension.EventBroker;
using Microsoft.Practices.Unity;

#endregion

namespace SimpleEventBroker
{
    /// <summary>   A published event. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public class PublishedEvent
    {
        public static class ContainerProvider
        {
            private static IUnityContainer _container;

            public static IUnityContainer EventBrokerContainer
            {
                get
                {
                    return _container ?? Container;
                }
                set { _container = value; }
            }
        }

        private static IUnityContainer Container { get; set; }

        private EventBroker Broker { get; }

        /// <summary>   The publishers. </summary>
        private readonly List<dynamic> publishers;

        /// <summary>   The subscribers. </summary>
        private readonly List<dynamic> subscribers;
        
        /// <summary>   The subscribers. </summary>
        private readonly List<Tuple<Type, SubscriptionInfo>> wakeupSubscribers;

        /// <summary>   Default constructor. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        public PublishedEvent(IUnityContainer container, EventBroker broker)
        {
            Container = container;
            Broker = broker;
            publishers = new List<dynamic>();
            subscribers = new List<dynamic>();
            wakeupSubscribers = new List<Tuple<Type, SubscriptionInfo>>();
        }

        /// <summary>   Gets the publishers. </summary>
        /// <value> The publishers. </value>
        public IEnumerable<object> Publishers
        {
            get
            {
                foreach ( var publisher in publishers )
                {
                    yield return publisher;
                }
            }
        }

        /// <summary>   Gets the subscribers. </summary>
        /// <value> The subscribers. </value>
        public IEnumerable<object> Subscribers
        {
            get
            {
                foreach ( var subscriber in subscribers )
                {
                    yield return subscriber;
                }
            }
        }

        public IEnumerable<dynamic> WakeupSubscribers
        {
            get
            {
                foreach ( var subscriber in wakeupSubscribers )
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
        public void AddPublisher<T>( object publisher, string eventName )
            where T : EventArgs
        {
            publishers.Add( publisher );
            var targetEvent = publisher.GetType().GetEvent( eventName );
            GuardEventExists( eventName, publisher, targetEvent );

            var addEventMethod = targetEvent.GetAddMethod();
            GuardAddMethodExists( targetEvent );

            EventHandler<T> newSubscriber = OnPublisherFiring;
            addEventMethod.Invoke( publisher, new object[] { newSubscriber } );
        }

        /// <summary>   Removes the publisher. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publisher">    The publisher. </param>
        /// <param name="eventName">    Name of the event. </param>
        public void RemovePublisher<T>( object publisher, string eventName )
            where T : EventArgs
        {
            publishers.Remove( publisher );
            var targetEvent = publisher.GetType().GetEvent( eventName );
            GuardEventExists( eventName, publisher, targetEvent );

            var removeEventMethod = targetEvent.GetRemoveMethod();
            GuardRemoveMethodExists( targetEvent );

            EventHandler<T> subscriber = OnPublisherFiring;
            removeEventMethod.Invoke( publisher, new object[] { subscriber } );
        }

        public void RemoveUnTypedPublisher( object publisher, string eventName )
        {
            var targetEvent = publisher.GetType().GetEvents()
                .FirstOrDefault(e=>e.IsDefined(typeof(PublishesAttribute)));
            GuardEventExists(eventName, publisher, targetEvent);

            var removePublisher = GetType().GetMethod( nameof( RemovePublisher ) );
            removePublisher.MakeGenericMethod(targetEvent.EventHandlerType.GenericTypeArguments[0])
                           .Invoke(this, new[] {publisher, targetEvent.Name});
        }

        private bool _resolvingWakeupSubscrivers;

        /// <summary>   Adds a subscriber. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="subscriber">   The subscriber. </param>
        public void AddSubscriber<T>( EventHandler<T> subscriber )
            where T : EventArgs
        {
            if (_resolvingWakeupSubscrivers)
            {
                return;
            }
            subscribers.Add( subscriber );
        }

        internal void AddWakeupSubscriber( Type declaringType, SubscriptionInfo sub )
        {
            if(wakeupSubscribers.Any(t=>t.Item1 == declaringType && t.Item2.Equals(sub)))
                return;
            wakeupSubscribers.Add( new Tuple<Type, SubscriptionInfo>( declaringType, sub ) );
        }

        /// <summary>   Removes the subscriber described by subscriber. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="subscriber">   The subscriber. </param>
        public void RemoveSubscriber<T>( EventHandler<T> subscriber )
            where T : EventArgs
        {
            subscribers.Remove( subscriber );
        }

        public void RemoveSubscriber( dynamic subscriber )
        {
            subscribers.Remove( subscriber );
        }

        /// <summary>   Raises the publisher firing event. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        private void OnPublisherFiring<T>( object sender, T e )
            where T : EventArgs
        {
            _resolvingWakeupSubscrivers = true;
            foreach ( var subscriber in wakeupSubscribers.ToArray() )
            {                
                // This will call another roundup of the builder and end up adding a new subscriber automatically
                var instance = Container.Resolve( subscriber.Item1 );
                var @delegate = Delegate.CreateDelegate( typeof( EventHandler<> ).MakeGenericType( subscriber.Item2.EventArgsType ), instance, subscriber.Item2.Subscriber );

                //var registerSubscriber = Broker.GetType().GetMethod( nameof( Broker.RegisterSubscriber ) );

                //// This will call another roundup of the builder and end up adding a new subscriber automatically
                //// Broker.RegisterSubscriber<T>(publishedEventName, delegate)
                //registerSubscriber.MakeGenericMethod( subscriber.Item2.EventArgsType )
                //                  .Invoke( Broker, new object[] { subscriber.Item2.PublishedEventName, @delegate } );
                
                subscribers.Add(@delegate);
            }
            _resolvingWakeupSubscrivers = false;

            foreach ( var subscriber in subscribers )
            {
                var sub = (EventHandler<T>)subscriber;
                sub( sender, e );
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
        private static void GuardEventExists( string eventName, object publisher, EventInfo targetEvent )
        {
            if ( targetEvent == null )
                throw new ArgumentException( string.Format( "The event '{0}' is not implemented on type '{1}'",
                        eventName,
                        publisher.GetType().Name ) );
        }

        /// <summary>   Queries if a given guard add method exists. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="ArgumentException">
        ///     Thrown when one or more arguments have unsupported or
        ///     illegal values.
        /// </exception>
        /// <param name="targetEvent">  Target event. </param>
        private static void GuardAddMethodExists( EventInfo targetEvent )
        {
            if ( targetEvent.GetAddMethod() == null )
                throw new ArgumentException( string.Format( "The event '{0}' does not have a public Add method",
                        targetEvent.Name ) );
        }

        /// <summary>   Queries if a given guard remove method exists. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="ArgumentException">
        ///     Thrown when one or more arguments have unsupported or
        ///     illegal values.
        /// </exception>
        /// <param name="targetEvent">  Target event. </param>
        private static void GuardRemoveMethodExists( EventInfo targetEvent )
        {
            if ( targetEvent.GetRemoveMethod() == null )
                throw new ArgumentException( string.Format( "The event '{0}' does not have a public Remove method",
                        targetEvent.Name ) );
        }
    }
}