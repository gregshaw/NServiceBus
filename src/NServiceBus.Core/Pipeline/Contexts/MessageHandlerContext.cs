﻿namespace NServiceBus.Pipeline.Contexts
{
    using Unicast.Behaviors;
    using Unicast.Messages;

    class MessageHandlerContext : BehaviorContext
    {
        public MessageHandlerContext(BehaviorContext parentContext, MessageHandler messageHandler)
            : base(parentContext)
        {
            Set(messageHandler);
        }

        public MessageHandler MessageHandler
        {
            get { return Get<MessageHandler>(); }
        }

        public LogicalMessage LogicalMessage
        {
            get { return Get<LogicalMessage>(); }
        }

        public TransportMessage PhysicalMessage
        {
            get { return Get<TransportMessage>(IncomingPhysicalMessageContext.IncomingPhysicalMessageKey); }
        }
    }
}