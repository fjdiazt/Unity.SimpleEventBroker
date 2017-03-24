using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventBrokerExtension.Providers
{
    /// <summary>
    /// The Awakable subscribers types provider
    /// </summary>
    public static class AwakableSubcribersProvider
    {
        private static IEnumerable<MethodInfo> _methods;

        /// <summary>
        /// Gets the subscribers.
        /// </summary>
        /// <value>
        /// The subscribers.
        /// </value>
        public static IEnumerable<MethodInfo> Subscribers =>
            _methods ?? ( _methods = GetAwakableSubscriberMethods() );

        /// <summary>
        /// Registers the awakable subscribers.
        /// </summary>
        /// <param name="methods">The methods.</param>
        public static void RegisterSubscribers( params MethodInfo[] methods )
        {
            if ( methods.Any( m => !m.IsDefined( typeof( SubscribesToAttribute ) ) ) )
                throw new Exception( $"One or more methods do not have a '{nameof( SubscribesToAttribute )}' attribute" );

            if ( methods.Any( m => m.GetCustomAttribute<SubscribesToAttribute>().Awake == false ) )
                throw new Exception( $"One or more method is not {nameof( SubscribesToAttribute.Awake )}" );

            _methods = methods;
        }

        private static IEnumerable<MethodInfo> GetAwakableSubscriberMethods()
        {
            return AppDomain.CurrentDomain
                            .GetAssemblies()
                            .SelectMany( GetLoadableTypes )
                            .SelectMany( t => t.GetMethods( BindingFlags.DeclaredOnly | BindingFlags.Public |
                                                           BindingFlags.Instance )
                                               .Where( m => m.IsDefined( typeof( SubscribesToAttribute ) ) ) );


        }

        private static IEnumerable<Type> GetLoadableTypes( Assembly assembly )
        {
            try
            {
                return assembly.GetTypes().Where( t => t.IsPublic && t.IsDefined( typeof( SubscriberAttribute ) ) );
            }
            catch ( ReflectionTypeLoadException e )
            {
                return e.Types.Where( t => t != null );
            }
        }
    }
}
