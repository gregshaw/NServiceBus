﻿namespace NServiceBus.Unicast.Messages
{
    using System;
    using System.Collections.Generic;

    class LogicalMessage
    {
        public LogicalMessage(MessageMetadata metadata, object message,Dictionary<string,string> headers)
        {
            Instance = message;
            Metadata = metadata;
            Headers = headers;
        }

        public void UpdateMessageInstance(object newMessage)
        {
            Instance = newMessage;
        }

        public Type MessageType
        {
            get
            {
                return Metadata.MessageType;
            }
        }

        public Dictionary<string, string> Headers { get; private set; }

        public MessageMetadata Metadata { get; private set; }

        public object Instance { get; private set; }
    }
}