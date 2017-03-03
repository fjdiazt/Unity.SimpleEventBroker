#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: EventBrokerReflectionStrategy.cs
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
    /// <summary>   An event broker reflection strategy. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public class EventBrokerReflectionStrategy : BuilderStrategy
    {
        private static IEnumerable<MethodInfo> WakeupEventsCache { get; set; }

        private bool WakeupEventsSubscribed { get; set; }

        /// <summary>   Pre build up. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="context">  The context. </param>
        public override void PreBuildUp(IBuilderContext context)
        {
            if(context.Policies.Get<IEventBrokerInfoPolicy>(context.BuildKey) == null)
            {
                var policy = new EventBrokerInfoPolicy();
                context.Policies.Set<IEventBrokerInfoPolicy>(policy, context.BuildKey);

                AddPublicationsToPolicy(context.BuildKey, policy);
                AddSubscriptionsToPolicy(context.BuildKey, policy);

                if (WakeupEventsSubscribed == false)
                    AddWakeupSubscriptionsToPolicy( context.BuildKey, policy );

                WakeupEventsSubscribed = true;
            }
        }

        // Handle subcribed types that are not yet instanced

        /// <summary>   Adds the publications to policy to 'policy'. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="buildKey"> The build key. </param>
        /// <param name="policy">   The policy. </param>
        private void AddPublicationsToPolicy(NamedTypeBuildKey buildKey, EventBrokerInfoPolicy policy)
        {
            if ( buildKey.Type.IsDefined( typeof( PublisherAttribute ) ) == false )
                return;

            var t = buildKey.Type;
            foreach(var eventInfo in t.GetEvents())
            {
                var attrs = (PublishesAttribute[])eventInfo.GetCustomAttributes(typeof(PublishesAttribute), true);
                foreach (var attr in attrs)
                {
                    policy.AddPublication(attr.EventName, eventInfo.Name);
                }
            }
        }

        /// <summary>   Adds the subscriptions to policy to 'policy'. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="buildKey"> The build key. </param>
        /// <param name="policy">   The policy. </param>
        private void AddSubscriptionsToPolicy(NamedTypeBuildKey buildKey, EventBrokerInfoPolicy policy)
        {
            if (buildKey.Type.IsDefined(typeof(SubscriberAttribute)) == false)
                return;

            var subscribedMethods = buildKey.Type.GetMethods().Where( m=>m.IsDefined( typeof(SubscribesToAttribute) ) ).ToArray();
            foreach (var method in subscribedMethods )
            {
                var attrs = (SubscribesToAttribute[])
                            method.GetCustomAttributes(typeof(SubscribesToAttribute), true);


                foreach (var attr in attrs)
                {
                    policy.AddSubscription(attr.EventName, method);
                }
            }
        }

        private void AddWakeupSubscriptionsToPolicy(NamedTypeBuildKey buildKey, EventBrokerInfoPolicy policy)
        {
            var subscribedMethods = buildKey.Type.GetMethods().Where( m => m.IsDefined( typeof( SubscribesToAttribute ) ) ).ToArray();
            
            foreach ( var method in GetWakeupSubscriberMethods( subscribedMethods ) )
            {
                var attrs = (SubscribesToAttribute[])method.GetCustomAttributes( typeof( SubscribesToAttribute ), true );

                foreach ( var attr in attrs )
                {
                    if ( attr?.WakeUp == true )
                        policy.AddSubscription( attr.EventName, method );
                }
            }
        }

        private IEnumerable<MethodInfo> GetWakeupSubscriberMethods(IEnumerable<MethodInfo> subscribedMethods )
        {
            if (WakeupEventsCache != null)
                return WakeupEventsCache;


            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            WakeupEventsCache = assemblies.SelectMany( GetLoadableTypes )
                .SelectMany( t => t.GetMethods( BindingFlags.DeclaredOnly | BindingFlags.Public |
                                               BindingFlags.Instance )
                                   .Where( m => m.IsDefined( typeof( SubscribesToAttribute ) ) ) )
                .Where( m => subscribedMethods.Contains( m ) == false );

            return WakeupEventsCache;
        }

        private static IEnumerable<Type> GetLoadableTypes( Assembly assembly )
        {
            try
            {
                return assembly.GetTypes().Where(t => t.IsPublic && t.IsDefined(typeof(SubscriberAttribute)));
            }
            catch ( ReflectionTypeLoadException e )
            {
                return e.Types.Where( t => t != null );
            }
        }
    }
}