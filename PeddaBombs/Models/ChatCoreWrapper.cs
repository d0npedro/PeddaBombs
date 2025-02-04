using CatCore;
using CatCore.Services.Multiplexer;
using CatCore.Services.Twitch.Interfaces;
using PeddaBombs.Events;
using System;
using System.Collections.Concurrent;
using Zenject;

namespace PeddaBombs.Models
{
    public class ChatCoreWrapper : IInitializable, IDisposable
    {
        public CatCoreInstance CoreInstance { get; private set; }
        public ChatServiceMultiplexer MultiplexerInstance { get; private set; }
        public ITwitchService TwitchService { get; private set; }
        public ConcurrentQueue<ReceiveMessageEventArgs> RecieveChatMessage { get; } = new ConcurrentQueue<ReceiveMessageEventArgs>();
        public ConcurrentQueue<string> SendMessageQueue { get; } = new ConcurrentQueue<string>();

        public event OnTextMessageReceivedHandler OnMessageReceived;

        private bool disposedValue;


        public void Initialize()
        {
            this.CoreInstance = CatCoreInstance.Create();
            this.MultiplexerInstance = this.CoreInstance.RunAllServices();
            this.TwitchService = this.MultiplexerInstance.GetTwitchPlatformService();
            this.MultiplexerInstance.OnTextMessageReceived += this.MultiplexerInstance_OnTextMessageReceived1; //this.MultiplexerInstance_OnTextMessageReceived;

            this.OnMessageReceived += this.ChatCoreWrapper_OnMessageReceived;
        }

        private void ChatCoreWrapper_OnMessageReceived(object sender, ReceiveMessageEventArgs e)
        {
            this.RecieveChatMessage.Enqueue(e);
        }
        private void MultiplexerInstance_OnTextMessageReceived1(MultiplexedPlatformService arg1, MultiplexedMessage arg2)
        {
            this.OnMessageReceived?.Invoke(this, new ReceiveMessageEventArgs(arg1, arg2));
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue) {
                if (disposing) {
                    this.MultiplexerInstance.OnTextMessageReceived += this.MultiplexerInstance_OnTextMessageReceived1;

                    this.OnMessageReceived -= this.ChatCoreWrapper_OnMessageReceived;
                }
                this.disposedValue = true;
            }
        }
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public delegate void OnTextMessageReceivedHandler(object sender, ReceiveMessageEventArgs e);
    public delegate void OnLoginHandler(object sender, LoginEventArgs e);
    public delegate void OnJoinChannelHandler(object sender, JoinChannelEventArgs e);
}
