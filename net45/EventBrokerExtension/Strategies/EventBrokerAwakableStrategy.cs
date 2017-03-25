using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Unity.EventBroker.Attributes;
using Unity.EventBroker.Providers;

namespace Unity.EventBroker.Strategies
{
    /// <summary>
    /// EventBrokerWakeupStrategy
    /// </summary>
    /// <seealso cref="Microsoft.Practices.ObjectBuilder2.BuilderStrategy" />
    public class EventBrokerAwakableStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp( IBuilderContext context )
        {
            if ( context.Policies.Get<IEventBrokerInfoPolicy>( context.BuildKey ) == null )
            {
                var policy = new EventBrokerInfoPolicy();
                context.Policies.Set<IEventBrokerInfoPolicy>( policy, context.BuildKey );

                AddAwakableSubscriptionsToPolicy( context.BuildKey, policy );
            }
        }

        private void AddAwakableSubscriptionsToPolicy( NamedTypeBuildKey buildKey, EventBrokerInfoPolicy policy )
        {
            var subscribedMethods = buildKey.Type.GetMethods().Where( m => CustomAttributeExtensions.IsDefined((MemberInfo) m, typeof( SubscribesToAttribute ) ) ).ToArray();

            var methods = AwakableSubcribersProvider.Subscribers.Where( m => !subscribedMethods.Contains( m ) );

            foreach ( var method in methods )
            {
                var attrs = (SubscribesToAttribute[])method.GetCustomAttributes( typeof( SubscribesToAttribute ), false );

                foreach ( var attr in attrs )
                {
                    if ( attr?.Awake == true )
                        policy.AddSubscription( attr.EventName, method );
                }
            }
        }
    }
}