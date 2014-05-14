#region File Header

// ***********************************************************************
// Author	: Sander Struijk
// File		: EventBrokerReflectionStrategy.cs
// Created	: 2014 05 14 13:09
// Updated	: 2014 05 14 13:21
// ***********************************************************************

#endregion

#region Using statements

using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using SimpleEventBroker;

#endregion

namespace EventBrokerExtension
{
    /// <summary>   An event broker reflection strategy. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public class EventBrokerReflectionStrategy : BuilderStrategy
    {
        /// <summary>
        ///     Called during the chain of responsibility for a build operation. The PreBuildUp
        ///     method is called when the chain is being executed in the forward direction.
        /// </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="context">  Context of the build operation. </param>
        public override void PreBuildUp(IBuilderContext context)
        {
            if(context.Policies.Get<IEventBrokerInfoPolicy>(context.BuildKey) == null)
            {
                var policy = new EventBrokerInfoPolicy();
                context.Policies.Set<IEventBrokerInfoPolicy>(policy, context.BuildKey);

                AddPublicationsToPolicy(context.BuildKey, policy);
                AddSubscriptionsToPolicy(context.BuildKey, policy);
            }
        }

        /// <summary>   Adds the publications to policy to 'policy'. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="buildKey"> The build key. </param>
        /// <param name="policy">   The policy. </param>
        private void AddPublicationsToPolicy(NamedTypeBuildKey buildKey, EventBrokerInfoPolicy policy)
        {
            var t = buildKey.Type;
            foreach(EventInfo eventInfo in t.GetEvents())
            {
                var attrs =
                        (PublishesAttribute[])eventInfo.GetCustomAttributes(typeof(PublishesAttribute), true);
                if(attrs.Length > 0)
                    policy.AddPublication(attrs[0].EventName, eventInfo.Name);
            }
        }

        /// <summary>   Adds the subscriptions to policy to 'policy'. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="buildKey"> The build key. </param>
        /// <param name="policy">   The policy. </param>
        private void AddSubscriptionsToPolicy(NamedTypeBuildKey buildKey, EventBrokerInfoPolicy policy)
        {
            foreach(MethodInfo method in buildKey.Type.GetMethods())
            {
                var attrs =
                        (SubscribesToAttribute[])
                                method.GetCustomAttributes(typeof(SubscribesToAttribute), true);
                if(attrs.Length > 0)
                    policy.AddSubscription(attrs[0].EventName, method);
            }
        }
    }
}