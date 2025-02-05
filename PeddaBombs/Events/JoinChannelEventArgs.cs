using CatCore.Services.Multiplexer;
using System;

namespace PeddaBombs.Events {
    public class JoinChannelEventArgs : EventArgs {
        public MultiplexedPlatformService ChatService { get; private set; }
        public MultiplexedChannel ChatChannel { get; private set; }
        public JoinChannelEventArgs(MultiplexedPlatformService service, MultiplexedChannel chatChannel) {
            this.ChatService = service;
            this.ChatChannel = chatChannel;
        }
    }
}
