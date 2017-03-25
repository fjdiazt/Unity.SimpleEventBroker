#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: SimpleEventBrokerExtension.cs
// Created	: 2014 05 14 13:09
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using Unity.EventBroker.Strategies;

#endregion

namespace Unity.EventBroker
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
        public EventBroker Broker { get; private set; }

        /// <summary>   Initial the container with this extension's functionality. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        protected override void Initialize()
        {
            Broker = new EventBroker();

            Context.Container.RegisterInstance( Broker, new TLifetimeManager() );

            //Context.Strategies.AddNew<EventBrokerWakeupStrategy>(UnityBuildStage.PreCreation);
            Context.Strategies.AddNew<EventBrokerReflectionStrategy>( UnityBuildStage.PreCreation );
            Context.Strategies.AddNew<EventBrokerWireupStrategy>( UnityBuildStage.Initialization );
        }
    }
}