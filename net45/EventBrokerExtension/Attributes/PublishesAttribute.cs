#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: PublishesAttribute.cs
// Created	: 2014 05 14 12:53
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using System;

#endregion

namespace Unity.EventBroker.Attributes
{
    /// <summary>   Attribute for publishes. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    [AttributeUsage(AttributeTargets.Event, AllowMultiple = true, Inherited = true )]
    public class PublishesAttribute : PublishSubscribeAttribute
    {
        /// <summary>   Constructor. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="publishedEventName">   Name of the published event. </param>
        public PublishesAttribute(string publishedEventName) : base(publishedEventName) {}
    }
}