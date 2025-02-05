using PeddaBombs.Interfaces;
using PeddaBombs.Models;
using System.Collections.Concurrent;
using UnityEngine;
using Zenject;

namespace PeddaBombs {
    public class PeddaBombsController : MonoBehaviour {
        public ConcurrentDictionary<string, ICommandable> CommandControllers { get; } = new ConcurrentDictionary<string, ICommandable>();
        private ChatCoreWrapper _chatCoreWrapper;

        [Inject]
        public void Constractor(DiContainer container, ChatCoreWrapper wrapper) {
            var controllers = container.ResolveAll<ICommandable>();
            foreach (var commandController in controllers) {
                _ = this.CommandControllers.TryAdd(commandController.Key, commandController);
            }
            this._chatCoreWrapper = wrapper;
        }

        private void Awake() {
            Plugin.Log?.Debug($"{this.name}: Awake()");
        }

        private void Update() {
            while (this._chatCoreWrapper.RecieveChatMessage.TryDequeue(out var message)) {
                if (string.IsNullOrEmpty(message.ChatMessage.Message)) {
                    continue;
                }
                if (this.CommandControllers.TryGetValue(message.ChatMessage.Message.Split(' ')[0], out var commandable)) {
                    commandable.Execute(message.ChatService, message.ChatMessage);
                    break;
                }
            }
        }

        private void OnDestroy() {
            Plugin.Log?.Debug($"{this.name}: OnDestroy()");
        }

    }
}
