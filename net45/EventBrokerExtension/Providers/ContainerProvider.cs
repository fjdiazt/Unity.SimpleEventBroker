using Microsoft.Practices.Unity;

namespace EventBrokerExtension.Providers
{
    /// <summary>
    /// The Unity Container provider to resolve types that can awake automatically.
    /// </summary>
    public static class ContainerProvider
    {
        /// <summary>
        /// Gets or sets the event broker container.
        /// </summary>
        /// <value>
        /// The event broker container.
        /// </value>
        public static IUnityContainer Current { get; private set; }

        /// <summary>
        /// Registers the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        public static void Register( IUnityContainer container )
        {
            Current = container;
        }
    }
}