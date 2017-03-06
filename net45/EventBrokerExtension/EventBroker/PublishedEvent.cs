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
using Microsoft.Practices.Unity;

#endregion

namespace EventBrokerExtension.EventBroker
{
    /// <summary>   A published event. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public class PublishedEvent
    {
        /// <summary>
        /// The Unity Container provider to resolve types that can awake automatically.
        /// </summary>
        public static class ContainerProvider
        {
            private static IUnityContainer _container;

            /// <summary>
            /// Gets or sets the event broker container.
            /// </summary>
            /// <value>
            /// The event broker container.
            /// </value>
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
        private readonly List<dynamic> _publishers;

        /// <summary>   The subscribers. </summary>
        private readonly List<dynamic> _subscribers;

        /// <summary>   The subscribers. </summary>
        private readonly List<Tuple<Type, SubscriptionInfo>> _wakeupSubscribers;

        private bool ResolvingWakeupSubscribers { get; set; }

        /// <summary>   Default constructor. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        public PublishedEvent( IUnityContainer container, EventBroker broker )
        {
            Container = container;
            Broker = broker;
            _publishers = new List<dynamic>();
            _subscribers = new List<dynamic>();
            _wakeupSubscribers = new List<Tuple<Type, SubscriptionInfo>>();
        }

        /// <summary>   Gets the publishers. </summary>
        /// <value> The publishers. </value>
        public IEnumerable<object> Publishers
        {
            get
            {
                foreach ( var publisher in _publishers )
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
                foreach ( var subscriber in _subscribers )
                {
                    yield return subscriber;
                }
            }
        }

        /// <summary>
        /// Gets the subscribers that can be awaken automatically.
        /// </summary>
        /// <value>
        /// The wakeup subscribers.
        /// </value>
        public IEnumerable<dynamic> WakeupSubscribers
        {
            get
            {
                foreach ( var subscriber in _wakeupSubscribers )
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
            get { return _publishers.Count > 0; }
        }

        /// <summary>
        ///     Gets a value indicating whether this SimpleEventBroker.PublishedEvent has
        ///     subscribers.
        /// </summary>
        /// <value> true if this SimpleEventBroker.PublishedEvent has subscribers, false if not. </value>
        public bool HasSubscribers
        {
            get { return _subscribers.Count > 0; }
        }

        /// <summary>   Adds a publisher to 'eventName'. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publisher">    The publisher. </param>
        /// <param name="eventName">    Name of the event. </param>
        public void AddPublisher<T>( object publisher, string eventName )
            where T : EventArgs
        {
            _publishers.Add( publisher );
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
            _publishers.Remove( publisher );
            var targetEvent = publisher.GetType().GetEvent( eventName );
            GuardEventExists( eventName, publisher, targetEvent );

            var removeEventMethod = targetEvent.GetRemoveMethod();
            GuardRemoveMethodExists( targetEvent );

            EventHandler<T> subscriber = OnPublisherFiring;
            removeEventMethod.Invoke( publisher, new object[] { subscriber } );
        }

        /// <summary>
        /// Removes the publisher unsafely, treating it as a dynamic type.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="eventName">Name of the event.</param>
        public void RemoveUnTypedPublisher( object publisher, string eventName )
        {
            var targetEvent = publisher.GetType().GetEvents()
                .FirstOrDefault( e => e.IsDefined( typeof( PublishesAttribute ) ) );
            GuardEventExists( eventName, publisher, targetEvent );

            var removePublisher = GetType().GetMethod( nameof( RemovePublisher ) );
            removePublisher.MakeGenericMethod( targetEvent.EventHandlerType.GenericTypeArguments[ 0 ] )
                           .Invoke( this, new[] { publisher, targetEvent.Name } );
        }

        /// <summary>   Adds a subscriber. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="subscriber">   The subscriber. </param>
        public void AddSubscriber<T>( EventHandler<T> subscriber )
            where T : EventArgs
        {
            if ( ResolvingWakeupSubscribers )
            {
                return;
            }
            _subscribers.Add( subscriber );
        }

        internal void AddWakeupSubscriber( Type declaringType, SubscriptionInfo sub )
        {
            if( _wakeupSubscribers.Any(t=>t.Item1 == declaringType && t.Item2.Equals(sub)))
                return;
            _wakeupSubscribers.Add( new Tuple<Type, SubscriptionInfo>( declaringType, sub ) );
        }

        /// <summary>   Removes the subscriber described by subscriber. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="subscriber">   The subscriber. </param>
        public void RemoveSubscriber<T>( EventHandler<T> subscriber )
            where T : EventArgs
        {
            _subscribers.Remove( subscriber );
        }

        public void RemoveSubscriber( dynamic subscriber )
        {
            _subscribers.Remove( subscriber );
        }

        /// <summary>   Raises the publisher firing event. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information to send to registered event handlers. </param>
        private void OnPublisherFiring<T>( object sender, T e )
            where T : EventArgs
        {
            AwakeSubscribers<T>();

            foreach ( var subscriber in _subscribers )
            {
                var sub = (EventHandler<T>)subscriber;
                sub( sender, e );
            }
        }

        private void AwakeSubscribers<T>()
            where T : EventArgs
        {
            ResolvingWakeupSubscribers = true;
            var subsAwake = _subscribers.Cast<EventHandler<T>>()
                                        .Select( s => new Tuple<Type, string>( s.Target.GetType(), s.Method.Name ) )
                                        .ToArray();

            foreach ( var subscriber in _wakeupSubscribers.Where( s => s.Item2.CanWakeUp && !s.Item2.IsAwake ) )
            {
                if ( subsAwake.Contains( new Tuple<Type, string>( subscriber.Item1, subscriber.Item2.Subscriber.Name ) ) )
                    continue;

                // This will call another roundup of the builder
                var instance = ContainerProvider.EventBrokerContainer.Resolve( subscriber.Item1 );

                // Add the instance manually to subscriber
                var @delegate = Delegate.CreateDelegate( typeof( EventHandler<> ).MakeGenericType( subscriber.Item2.EventArgsType ), instance, subscriber.Item2.Subscriber );

                var registerSubscriber = Broker.GetType().GetMethod( nameof( Broker.RegisterSubscriber ) );

                registerSubscriber.MakeGenericMethod( subscriber.Item2.EventArgsType )
                                  .Invoke( Broker, new object[] { subscriber.Item2.PublishedEventName, @delegate } );

                _subscribers.Add( @delegate );
            }
            ResolvingWakeupSubscribers = false;
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
                throw new ArgumentException(
                    $"The event '{eventName}' is not implemented on type '{publisher.GetType().Name}'");
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
                throw new ArgumentException($"The event '{targetEvent.Name}' does not have a public Add method");
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
                throw new ArgumentException($"The event '{targetEvent.Name}' does not have a public Remove method");
        }
    }
}