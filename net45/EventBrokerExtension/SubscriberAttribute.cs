﻿using System;

namespace EventBrokerExtension
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
    public class SubscriberAttribute : Attribute
    {        
    }
}
