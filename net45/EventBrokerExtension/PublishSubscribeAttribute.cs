#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: PublishSubscribeAttribute.cs
// Created	: 2014 05 14 12:53
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using System;

#endregion

namespace SimpleEventBroker
{
    /// <summary>
    ///     Base class for the two publish / subscribe attributes. Stores the event name to
    ///     be published or subscribed to.
    /// </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public abstract class PublishSubscribeAttribute : Attribute
    {
        /// <summary>   Specialised constructor for use only by derived classes. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="eventName">    The name of the event. </param>
        protected PublishSubscribeAttribute(string eventName)
        {
            EventName = eventName;
        }

        /// <summary>   Gets or sets the name of the event. </summary>
        /// <value> The name of the event. </value>
        public string EventName { get; set; }
    }
}