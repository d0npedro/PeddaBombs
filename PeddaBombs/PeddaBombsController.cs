using CatCore.Services.Multiplexer;
using PeddaBombs.Configuration;
using PeddaBombs.Models;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;
using Newtonsoft.Json;
using System;

namespace PeddaBombs {
    public class PeddaBombsController : MonoBehaviour {
        private ChatCoreWrapper _chatCoreWrapper;
        private List<Command> _commands;

        [Inject]
        public void Constractor(ChatCoreWrapper wrapper) {
            this._chatCoreWrapper = wrapper;
            this._commands = CommandLoader.LoadCommands(Path.Combine(Environment.CurrentDirectory, "UserData", "PeddaBombsCommands.json"));
        }

        private void Awake() {
            Plugin.Log?.Debug($"{this.name}: Awake()");
        }

        private void Update() {
            while (this._chatCoreWrapper.RecieveChatMessage.TryDequeue(out var message)) {
                if (string.IsNullOrEmpty(message.ChatMessage.Message)) {
                    continue;
                }

                var command = message.ChatMessage.Message.Split(' ')[0];
                if (command == "!bomb") {
                    ExecuteBombCommand(message.ChatService, message.ChatMessage);
                }
                foreach (var cmd in this._commands) {
                    if (command == cmd.CommandText) {
                        ExecuteBombCommandWithText(message.ChatService, message.ChatMessage, cmd.ResponseText);
                        break;
                    }
                }
            }
        }

        private void ExecuteBombCommand(MultiplexedPlatformService service, MultiplexedMessage message) {
            if (PluginConfig.Instance.IsBombEnable != true) {
                return;
            }
            DummyBomb.Senders.Enqueue(message.Sender.DisplayName);
        }

        private void ExecuteBombCommandWithText(MultiplexedPlatformService service, MultiplexedMessage message, string textMessage) {
            if (PluginConfig.Instance.IsBombEnable != true) {
                return;
            }
            DummyBomb.Senders.Enqueue(textMessage);
        }

        private void OnDestroy() {
            Plugin.Log?.Debug($"{this.name}: OnDestroy()");
        }
    }

    public class Command {
        public string CommandText { get; set; }
        public string ResponseText { get; set; }
    }

    public static class CommandLoader {
        public static List<Command> LoadCommands(string filePath) {
            try {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<Command>>(json);
            } catch (JsonSerializationException ex) {
                Plugin.Log?.Critical($"Failed to deserialize commands from {filePath}: {ex.Message}");
                return new List<Command>();
            } catch (Exception ex) {
                Plugin.Log?.Critical($"An error occurred while loading commands from {filePath}: {ex.Message}");
                return new List<Command>();
            }
        }
    }
}
