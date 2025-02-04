using PeddaBombs.Interfaces;
using PeddaBombs.Models;
using System.Collections.Concurrent;
using UnityEngine;
using Zenject;

namespace PeddaBombs
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class PeddaBombsController : MonoBehaviour
    {

        public ConcurrentDictionary<string, ICommandable> CommandControllers { get; } = new ConcurrentDictionary<string, ICommandable>();
    
        private ChatCoreWrapper _chatCoreWrapper;

        [Inject]
        public void Constractor(DiContainer container, ChatCoreWrapper wrapper)
        {
            var controllers = container.ResolveAll<ICommandable>();
            foreach (var commandController in controllers) {
                _ = this.CommandControllers.TryAdd(commandController.Key, commandController);
            }
            this._chatCoreWrapper = wrapper;
        }

        
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake()
        {
            Plugin.Log?.Debug($"{this.name}: Awake()");
        }

        /// <summary>
        /// Called every frame if the script is enabled.
        /// </summary>
        private void Update()
        {
            while (this._chatCoreWrapper.RecieveChatMessage.TryDequeue(out var message)) {
                if (string.IsNullOrEmpty(message.ChatMessage.Message)) {
                    continue;
                }
                if (this.CommandControllers.TryGetValue(message.ChatMessage.Message.Split(' ')[0], out var commandable)) {
                    // 1F1コマンド
                    commandable.Execute(message.ChatService, message.ChatMessage);
                    break;
                }
            }
        }
        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{this.name}: OnDestroy()");
        }

    }
}
