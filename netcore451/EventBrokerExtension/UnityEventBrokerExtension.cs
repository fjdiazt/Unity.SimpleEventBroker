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
using SimpleEventBroker;

#endregion

namespace EventBrokerExtension
{
    public class UnityEventBrokerExtension: UnityEventBrokerExtension<ExternallyControlledLifetimeManager>
    {
    }

    /// <summary>   A simple event broker extension. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public class UnityEventBrokerExtension<TLifetimeManager> : UnityContainerExtension, ISimpleEventBrokerConfiguration
        where TLifetimeManager : LifetimeManager, new()
    {
        /// <summary>   Initial the container with this extension's functionality. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        protected override void Initialize()
        {
            Context.Container.RegisterInstance(broker, new TLifetimeManager());

            Context.Strategies.AddNew<EventBrokerReflectionStrategy>(UnityBuildStage.PreCreation);
            Context.Strategies.AddNew<EventBrokerWireupStrategy>(UnityBuildStage.Initialization);
        }

        /// <summary>   The broker. </summary>
        private readonly EventBroker broker = new EventBroker();

        /// <summary>   Gets the broker. </summary>
        /// <value> The broker. </value>
        public EventBroker Broker
        {
            get { return broker; }
        }
    }
}