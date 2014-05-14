#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: SimpleEventBrokerExtension.cs
// Created	: 2014 05 14 12:53
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
    /// <summary>   A simple event broker extension. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public class SimpleEventBrokerExtension : UnityContainerExtension, ISimpleEventBrokerConfiguration
    {
        /// <summary>   The broker. </summary>
        private readonly EventBroker broker = new EventBroker();

        /// <summary>   Gets the broker. </summary>
        /// <value> The broker. </value>
        public EventBroker Broker
        {
            get { return broker; }
        }

        /// <summary>   Initializes this EventBrokerExtension.SimpleEventBrokerExtension. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        protected override void Initialize()
        {
            Context.Container.RegisterInstance(broker, new ExternallyControlledLifetimeManager());

            Context.Strategies.AddNew<EventBrokerReflectionStrategy>(UnityBuildStage.PreCreation);
            Context.Strategies.AddNew<EventBrokerWireupStrategy>(UnityBuildStage.Initialization);
        }
    }
}