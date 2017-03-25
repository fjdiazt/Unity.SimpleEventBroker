﻿#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: EventBrokerReflectionStrategy.cs
// Created	: 2014 05 14 12:53
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Unity.EventBroker.Attributes;
using Unity.EventBroker.Providers;

#endregion

namespace Unity.EventBroker.Strategies
{
    /// <summary>   An event broker reflection strategy. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public class EventBrokerReflectionStrategy : BuilderStrategy
    {
        /// <summary>   Pre build up. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="context">  The context. </param>
        public override void PreBuildUp( IBuilderContext context )
        {
            if ( context.BuildKey == null )
                return;

            if ( context.BuildKey?.Type.IsDefined( typeof( PublisherAttribute ), false ) == false
                 && context.BuildKey?.Type.IsDefined( typeof( SubscriberAttribute ), false ) == false )
                return;

            if ( context.Policies.Get<IEventBrokerInfoPolicy>( context.BuildKey ) == null )
            {
                var policy = new EventBrokerInfoPolicy();
                context.Policies.Set<IEventBrokerInfoPolicy>( policy, context.BuildKey );

                AddPublicationsToPolicy( context.BuildKey, policy );

                var awakeSubscribers = AddSubscriptionsToPolicy( context.BuildKey, policy );

                AddWakeupSubscriptionsToPolicy( context.BuildKey, policy, awakeSubscribers );
            }
        }

        //public override void PostBuildUp( IBuilderContext context )
        //{
        //    base.PostBuildUp( context );
        //}

        /// <summary>   Adds the publications to policy to 'policy'. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="buildKey"> The build key. </param>
        /// <param name="policy">   The policy. </param>
        private static void AddPublicationsToPolicy( NamedTypeBuildKey buildKey, EventBrokerInfoPolicy policy )
        {
            if ( buildKey.Type.IsDefined( typeof( PublisherAttribute ) ) == false )
                return;

            var t = buildKey.Type;
            foreach ( var eventInfo in t.GetEvents() )
            {
                var attrs = (PublishesAttribute[])eventInfo.GetCustomAttributes( typeof( PublishesAttribute ), true );
                foreach ( var attr in attrs )
                {
                    policy.AddPublication( attr.EventName, eventInfo.Name );
                }
            }
        }

        /// <summary>   Adds the subscriptions to policy to 'policy'. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="buildKey"> The build key. </param>
        /// <param name="policy">   The policy. </param>
        private static IEnumerable<SubscriptionInfo> AddSubscriptionsToPolicy( NamedTypeBuildKey buildKey,
                                                                               EventBrokerInfoPolicy policy )
        {
            if ( buildKey.Type.IsDefined( typeof( SubscriberAttribute ) ) == false )
                return new SubscriptionInfo[ 0 ];

            var awakeMethods = new List<SubscriptionInfo>();

            var subscribedMethods = buildKey.Type.GetMethods().Where( m => m.IsDefined( typeof( SubscribesToAttribute ) ) ).ToArray();
            foreach ( var method in subscribedMethods )
            {
                var attrs = (SubscribesToAttribute[])
                            method.GetCustomAttributes( typeof( SubscribesToAttribute ), true );


                foreach ( var attr in attrs )
                {
                    var subInfo = policy.AddSubscription( attr.EventName, method, isAwake: true );
                    awakeMethods.Add( subInfo );
                }
            }

            return awakeMethods;
        }

        private void AddWakeupSubscriptionsToPolicy( NamedTypeBuildKey buildKey, EventBrokerInfoPolicy policy,
            IEnumerable<SubscriptionInfo> awakeMethods )
        {
            if ( buildKey.Type.IsDefined( typeof( PublisherAttribute ), false ) == false )
                return;

            var publishedNames = buildKey
                .Type
                .GetEvents()
                .Where( m => m.IsDefined( typeof( PublishesAttribute ) ) )
                .SelectMany( a => (PublishesAttribute[])a.GetCustomAttributes( typeof( PublishesAttribute ), false ) )
                .Select( a => a.EventName );

            var methods = AwakableSubcribersProvider.Subscribers
                .Where( m => ( (SubscribesToAttribute[])m.GetCustomAttributes( typeof( SubscribesToAttribute ),
                                                                              false ) )
                                 .Any( a => publishedNames.Contains( a.EventName ) ) )
                .Where( m => awakeMethods.Any( a => a.Subscriber.Name == m.Name ) == false );

            foreach ( var method in methods )
            {
                var attrs = method.GetCustomAttributes<SubscribesToAttribute>( false );

                foreach ( var attr in attrs )
                {
                    var isNew = attr.Awake &&
                                !policy.Subscriptions
                                       .Any( p => p.Subscriber.Name == method.Name &&
                                                  p.PublishedEventName == attr.EventName &&
                                                  p.Subscriber.DeclaringType == method.DeclaringType &&
                                                  p.IsAwake == false );
                    if ( isNew )
                    {
                        policy.AddSubscription( attr.EventName, method, attr.Awake );
                    }
                }
            }
        }
    }
}