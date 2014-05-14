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

namespace SimpleEventBroker
{
    /// <summary>   Attribute for subscribes to. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class SubscribesToAttribute : PublishSubscribeAttribute
    {
        /// <summary>   Constructor. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="eventName">    Name of the event. </param>
        public SubscribesToAttribute(string eventName) : base(eventName) {}
    }
}