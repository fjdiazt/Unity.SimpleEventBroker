#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: SimpleEventBrokerExtension.cs
// Created	: 2014 05 14 13:09
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using System.Linq;
using System.Reflection;
using EventBrokerExtension.Providers;
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

    /// <summary>
    /// EventBrokerWakeupStrategy
    /// </summary>
    /// <seealso cref="Microsoft.Practices.ObjectBuilder2.BuilderStrategy" />
    public class EventBrokerWakeupStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp( IBuilderContext context )
        {
            if ( context.Policies.Get<IEventBrokerInfoPolicy>( context.BuildKey ) == null )
            {
                var policy = new EventBrokerInfoPolicy();
                context.Policies.Set<IEventBrokerInfoPolicy>( policy, context.BuildKey );

                AddAwakableSubscriptionsToPolicy( context.BuildKey, policy );
            }
        }

        private void AddAwakableSubscriptionsToPolicy( NamedTypeBuildKey buildKey, EventBrokerInfoPolicy policy )
        {
            var subscribedMethods = buildKey.Type.GetMethods().Where( m => m.IsDefined( typeof( SubscribesToAttribute ) ) ).ToArray();

            var methods = AwakableSubcribersProvider.Subscribers.Where( m => !subscribedMethods.Contains( m ) );

            foreach ( var method in methods )
            {
                var attrs = (SubscribesToAttribute[])method.GetCustomAttributes( typeof( SubscribesToAttribute ), false );

                foreach ( var attr in attrs )
                {
                    if ( attr?.Awake == true )
                        policy.AddSubscription( attr.EventName, method );
                }
            }
        }
    }

}