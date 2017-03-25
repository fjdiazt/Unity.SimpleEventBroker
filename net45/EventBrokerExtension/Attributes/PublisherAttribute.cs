using System;

namespace Unity.EventBroker.Attributes
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
    public class PublisherAttribute : Attribute
    {
    }
}
