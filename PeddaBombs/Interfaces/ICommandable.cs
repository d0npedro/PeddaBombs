using CatCore.Services.Multiplexer;

namespace PeddaBombs.Interfaces {
    public interface ICommandable {
        string Key { get; }
        void Execute(MultiplexedPlatformService service, MultiplexedMessage message);
    }
}
