using CatCore.Services.Multiplexer;

namespace PeddaBombs.Events {
    public class LoginEventArgs {
        public MultiplexedPlatformService ChatService { get; private set; }

        public LoginEventArgs(MultiplexedPlatformService service) {
            this.ChatService = service;
        }
    }
}
