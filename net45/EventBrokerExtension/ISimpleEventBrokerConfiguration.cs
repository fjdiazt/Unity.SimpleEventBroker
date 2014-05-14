#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: ISimpleEventBrokerConfiguration.cs
// Created	: 2014 05 14 12:53
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using Microsoft.Practices.Unity;
using SimpleEventBroker;

#endregion

namespace EventBrokerExtension
{
    /// <summary>   Interface for simple event broker configuration. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public interface ISimpleEventBrokerConfiguration : IUnityContainerExtensionConfigurator
    {
        /// <summary>   Gets the broker. </summary>
        /// <value> The broker. </value>
        EventBroker Broker { get; }
    }
}