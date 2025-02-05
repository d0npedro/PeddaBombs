using PeddaBombs.Interfaces;
using PeddaBombs.Models;
using System.Collections.Concurrent;
using UnityEngine;
using Zenject;

namespace PeddaBombs
{
    // Dieser Controller verwaltet alle Chat-Command-Controller und verarbeitet eingehende Chat-Nachrichten.
    public class PeddaBombsController : MonoBehaviour
    {
        // Ein Thread-sicheres Dictionary, in dem alle ICommandable-Instanzen gespeichert werden.
        // Der Schlüssel ist der Command-String (z. B. "!bomb", "!rofl" etc.),
        // der Wert ist der entsprechende Controller, der den Befehl verarbeitet.
        public ConcurrentDictionary<string, ICommandable> CommandControllers { get; } = new ConcurrentDictionary<string, ICommandable>();

        // Eine Referenz zum ChatCoreWrapper, der für den Empfang und die Verwaltung von Chat-Nachrichten zuständig ist.
        private ChatCoreWrapper _chatCoreWrapper;

        // Konstruktor-Methode, die von Zenject (Dependency Injection) aufgerufen wird.
        // Hier werden alle ICommandable-Implementierungen aus dem DI-Container aufgelöst
        // und in das CommandControllers-Dictionary eingefügt.
        [Inject]
        public void Constractor(DiContainer container, ChatCoreWrapper wrapper)
        {
            // Alle Objekte, die das Interface ICommandable implementieren, werden aus dem Container abgerufen.
            var controllers = container.ResolveAll<ICommandable>();
            // Jeder gefundene Command-Controller wird im Dictionary gespeichert, wobei sein Key (z. B. "!bomb") verwendet wird.
            foreach (var commandController in controllers)
            {
                _ = this.CommandControllers.TryAdd(commandController.Key, commandController);
            }
            // Speichert die ChatCoreWrapper-Instanz zur späteren Verwendung.
            this._chatCoreWrapper = wrapper;
        }

        // Die Awake-Methode wird von Unity aufgerufen, sobald das GameObject instanziert wird.
        private void Awake()
        {
            // Loggt eine Debug-Nachricht, um anzuzeigen, dass das Objekt geweckt wurde.
            Plugin.Log?.Debug($"{this.name}: Awake()");
        }

        // Die Update-Methode wird einmal pro Frame aufgerufen.
        private void Update()
        {
            // Verarbeitet alle Nachrichten, die im ChatCoreWrapper in der Warteschlange liegen.
            // TryDequeue entfernt eine Nachricht aus der ConcurrentQueue, falls vorhanden.
            while (this._chatCoreWrapper.RecieveChatMessage.TryDequeue(out var message))
            {
                // Falls die empfangene Nachricht leer oder null ist, wird sie übersprungen.
                if (string.IsNullOrEmpty(message.ChatMessage.Message))
                {
                    continue;
                }
                // Extrahiert den ersten Teil der Nachricht (angenommen, es handelt sich um den Command, z. B. "!bomb").
                // Split(' ') teilt die Nachricht an Leerzeichen.
                if (this.CommandControllers.TryGetValue(message.ChatMessage.Message.Split(' ')[0], out var commandable))
                {
                    // Führt den gefundenen Command aus, indem der Chat-Service und die ChatMessage übergeben werden.
                    commandable.Execute(message.ChatService, message.ChatMessage);
                    // Sobald ein Command pro Frame verarbeitet wurde, wird die Schleife unterbrochen.
                    break;
                }
            }
        }

        // Die OnDestroy-Methode wird von Unity aufgerufen, wenn das GameObject zerstört wird.
        private void OnDestroy()
        {
            // Loggt eine Debug-Nachricht, um anzuzeigen, dass das Objekt zerstört wird.
            Plugin.Log?.Debug($"{this.name}: OnDestroy()");
        }
    }
}
