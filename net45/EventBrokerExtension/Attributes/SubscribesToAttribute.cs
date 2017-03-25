#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: SubscribesToAttribute.cs
// Created	: 2014 05 14 12:53
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using System;

#endregion

namespace Unity.EventBroker.Attributes
{
    /// <summary>   Attribute for subscribes to. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    [AttributeUsage( AttributeTargets.Method, AllowMultiple = true, Inherited = true )]
    public class SubscribesToAttribute : PublishSubscribeAttribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether to wake up to an event 
        /// even if there is no existing instance of this subscriber.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [wake up]; otherwise, <c>false</c>.
        /// </value>
        public bool Awake { get; set; }

        /// <summary>   Constructor. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="eventName">    Name of the event. </param>
        public SubscribesToAttribute( string eventName ) : base( eventName ) { }
    }
}