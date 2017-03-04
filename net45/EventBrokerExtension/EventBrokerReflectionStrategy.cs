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

        /// <summary>   Pre build up. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="context">  The context. </param>
        public override void PreBuildUp(IBuilderContext context)
        {
            if ( context.BuildKey?.Type.IsDefined( typeof( PublisherAttribute ), false ) == false
                 && context.BuildKey?.Type.IsDefined( typeof( SubscriberAttribute ), false ) == false )
                return;

            if (context.Policies.Get<IEventBrokerInfoPolicy>(context.BuildKey) == null)
            {
                var policy = new EventBrokerInfoPolicy();
                context.Policies.Set<IEventBrokerInfoPolicy>(policy, context.BuildKey);

                AddPublicationsToPolicy(context.BuildKey, policy);
                var awakeSubscribers = AddSubscriptionsToPolicy(context.BuildKey, policy);
                //AddWakeupSubscriptionsToPolicy( context.BuildKey, policy, awakeSubscribers );
            }
        }

        public override void PostBuildUp(IBuilderContext context)
        {            
            base.PostBuildUp(context);
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
        private IEnumerable<MethodInfo> AddSubscriptionsToPolicy(NamedTypeBuildKey buildKey, EventBrokerInfoPolicy policy)
        {
            if (buildKey.Type.IsDefined(typeof(SubscriberAttribute)) == false)
                return new MethodInfo[0];

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

            return subscribedMethods;
        }

        private void AddWakeupSubscriptionsToPolicy(NamedTypeBuildKey buildKey, EventBrokerInfoPolicy policy,
            IEnumerable<MethodInfo> awakeMethods)
        {
            if(buildKey.Type.IsDefined(typeof(PublisherAttribute), false) == false)
                return;

            var publishedNames = buildKey
                .Type
                .GetEvents()
                .Where(m => m.IsDefined(typeof(PublishesAttribute)))
                .SelectMany(a => (PublishesAttribute[]) a.GetCustomAttributes(typeof(PublishesAttribute), false))
                .Select(a => a.EventName);

            var methods = GetWakeupSubscriberMethods()
                .Where( m => ((SubscribesToAttribute[])m.GetCustomAttributes( typeof( SubscribesToAttribute ), true ))                    
                .Any(a=>publishedNames.Contains(a.EventName)))
                .Where(m=>awakeMethods.Contains(m) == false);

            foreach ( var method in  methods)
            {
                var attrs = method.GetCustomAttributes<SubscribesToAttribute>(true);

                foreach ( var attr in attrs )
                {
                    if(attr.WakeUp)
                        policy.AddSubscription( attr.EventName, method );
                }
            }
        }

        private IEnumerable<MethodInfo> GetWakeupSubscriberMethods()
        {
            if (WakeupEventsCache != null)
                return WakeupEventsCache;

            WakeupEventsCache = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(GetLoadableTypes)
                .SelectMany(t => t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public |
                                              BindingFlags.Instance)
                                  .Where(m => m.IsDefined(typeof(SubscribesToAttribute))));

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