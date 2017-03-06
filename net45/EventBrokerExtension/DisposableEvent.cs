namespace EventBrokerExtension
{
    /// <summary>
    /// An event that can be flagged to be unsubscribed automatically by the Event Broker
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IUnsubscribableEvent
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="IUnsubscribableEvent"/> is unsubscribed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if un-subscribe; otherwise, <c>false</c>.
        /// </value>
        bool Unsubscribe { get; }
    }
}
