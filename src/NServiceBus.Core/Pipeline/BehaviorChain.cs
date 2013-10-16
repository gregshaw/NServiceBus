﻿namespace NServiceBus.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Logging;
    using ObjectBuilder;

    class BehaviorChain
    {
        static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        IBuilder builder;
        List<BehaviorChainItemDescriptor> items = new List<BehaviorChainItemDescriptor>();

        public BehaviorChain(IBuilder builder)
        {
            this.builder = builder;
        }

        /// <summary>
        /// Adds the given behavior to the chain
        /// </summary>
        public void Add<TBehavior>() where TBehavior : IBehavior
        {
            items.Add(new BehaviorChainItemDescriptor(typeof(TBehavior), builder));
        }

        /// <summary>
        /// Adds the given behavior to the chain, allowing the caller to initialize it before it is invoked
        /// </summary>
        public void Add<TBehavior>(Action<TBehavior> init) where TBehavior : IBehavior
        {
            items.Add(new BehaviorChainItemDescriptor(typeof(TBehavior), builder, init));
        }

        public void Invoke(TransportMessage incomingTransportMessage)
        {
            var head = GenerateBehaviorChain();
            
            using (var context = new BehaviorContext(incomingTransportMessage))
            {
                try
                {
                    head.Invoke(context);
                }
                catch (Exception exception)
                {
                    var error = string.Format("An error occurred while attempting to invoke the following behavior chain: {0}",string.Join(" -> ", items));
                    throw new Exception(error, exception);
                }
                finally
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug(context.GetTrace());
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine,
                               items.Select((type, index) => new string(' ', index * 2) + " -> " + type));
        }

        IBehavior GenerateBehaviorChain()
        {
            var clonedList = items.ToList();
            clonedList.Reverse();

            // start with the end
            IBehavior behavior = new Terminator();

            // traverse the pipeline in reverse order, tying each behavior to the following
            foreach (var type in clonedList)
            {
                var next = behavior;
                behavior = type.GetInstance();
                behavior.Next = next;
            }
            return behavior;
        }


    }
}