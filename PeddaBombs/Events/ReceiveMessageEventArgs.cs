using CatCore.Services.Multiplexer;
using System;

namespace PeddaBombs.Events
{
    public class ReceiveMessageEventArgs : EventArgs
    {
        public MultiplexedPlatformService ChatService { get; private set; }
        public MultiplexedMessage ChatMessage { get; private set; }

        public ReceiveMessageEventArgs(MultiplexedPlatformService service, MultiplexedMessage chatMessage)
        {
            this.ChatService = service;
            this.ChatMessage = chatMessage;
        }
    }
}
