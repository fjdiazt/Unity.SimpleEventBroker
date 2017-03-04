using System;

namespace EventBrokerExtension
{
    public interface IDisposableEvent : IDisposable
    {
        bool IsDisposed { get; }
    }
}
