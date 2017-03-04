#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: SimpleEventBrokerExtension.cs
// Created	: 2014 05 14 13:09
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

#endregion

namespace EventBrokerExtension
{
    /// <summary>   A simple event broker extension. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public class UnityEventBrokerExtension : UnityEventBrokerExtension<ExternallyControlledLifetimeManager>
    {
    }

    /// <summary>   A simple event broker extension. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public class UnityEventBrokerExtension<TLifetimeManager> : UnityContainerExtension, ISimpleEventBrokerConfiguration
        where TLifetimeManager : LifetimeManager, new()
    {
        /// <summary>   Gets the broker. </summary>
        /// <value> The broker. </value>
        public EventBroker.EventBroker Broker { get; private set; }

        /// <summary>   Initial the container with this extension's functionality. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        protected override void Initialize()
        {
            Broker = new EventBroker.EventBroker( Context.Container );

            Context.Container.RegisterInstance( Broker, new TLifetimeManager() );

            //Context.Strategies.AddNew<EventBrokerWakeupStrategy>(UnityBuildStage.PreCreation);
            Context.Strategies.AddNew<EventBrokerReflectionStrategy>( UnityBuildStage.PreCreation );
            Context.Strategies.AddNew<EventBrokerWireupStrategy>( UnityBuildStage.Initialization );
        }
    }

    public class EventBrokerWakeupStrategy : BuilderStrategy
    {
        private static IEnumerable<MethodInfo> WakeupEventsCache { get; set; }

        public override void PreBuildUp(IBuilderContext context)
        {
            if ( context.Policies.Get<IEventBrokerInfoPolicy>( context.BuildKey ) == null )
            {
                var policy = new EventBrokerInfoPolicy();
                context.Policies.Set<IEventBrokerInfoPolicy>( policy, context.BuildKey );

                AddWakeupSubscriptionsToPolicy( context.BuildKey, policy );
            }
        }

        private void AddWakeupSubscriptionsToPolicy( NamedTypeBuildKey buildKey, EventBrokerInfoPolicy policy )
        {
            var subscribedMethods = buildKey.Type.GetMethods().Where( m => m.IsDefined( typeof( SubscribesToAttribute ) ) ).ToArray();

            var methods = GetWakeupSubscriberMethods( subscribedMethods );

            foreach ( var method in methods )
            {
                var attrs = (SubscribesToAttribute[])method.GetCustomAttributes( typeof( SubscribesToAttribute ), false );

                foreach ( var attr in attrs )
                {
                    if ( attr?.WakeUp == true )
                        policy.AddSubscription( attr.EventName, method );
                }
            }
        }

        private IEnumerable<MethodInfo> GetWakeupSubscriberMethods( IEnumerable<MethodInfo> subscribedMethods )
        {
            if ( WakeupEventsCache != null )
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
                return assembly.GetTypes().Where( t => t.IsPublic && t.IsDefined( typeof( SubscriberAttribute ) ) );
            }
            catch ( ReflectionTypeLoadException e )
            {
                return e.Types.Where( t => t != null );
            }
        }
    }

}