using System.Linq; // LINQ wird benötigt, um in der Liste zu suchen.
using CatCore.Services.Multiplexer;
using PeddaBombs.Configuration;
using PeddaBombs.Interfaces;
using PeddaBombs.Models;
using PeddaBombs.Statics;
using UnityEngine;

namespace PeddaBombs.CommandControllers
{
    // Dieser Controller verarbeitet alle bombenbezogenen Befehle.
    // Er wird (idealerweise) für den Standardbefehl "!bomb" sowie für alle in der Konfiguration
    // definierten Bombenbefehle (z. B. "!rofl", "!awesome" etc.) registriert.
    public class BombCommandController : MonoBehaviour, ICommandable
    {
        // Für die Registrierung im Dispatcher wird hier ein Standard-Key verwendet.
        // Wichtig: Damit dieser Controller auch für weitere Bombenbefehle aufgerufen wird,
        // muss entweder der Dispatcher so erweitert werden oder es wird beim Plugin-Start
        // für jeden zusätzlichen Befehl ein Eintrag mit diesem Controller registriert.
        public string Key => CommandKey.BOMB;

        public void Execute(MultiplexedPlatformService service, MultiplexedMessage message)
        {
            // Prüfe, ob Bomben aktiviert sind.
            if (!PluginConfig.Instance.IsBombEnable)
                return;

            // Lies den gesamten Chatbefehl ein (z. B. "!bomb", "!rofl", etc.).
            string command = message.Message.Trim();

            // Standardbefehl: Wenn der Befehl genau "!bomb" lautet, verwende den Anzeigenamen.
            if (command.Equals(CommandKey.BOMB, System.StringComparison.OrdinalIgnoreCase))
            {
                DummyBomb.Senders.Enqueue(message.Sender.DisplayName);
                return;
            }
            else
            {
                // Suche in der Liste der Bombenbefehle nach einem Eintrag, der dem eingegebenen Befehl entspricht.
                // Hier nutzen wir LINQ, um das erste passende BombCommand-Objekt zu finden.
                var bombCmd = PluginConfig.Instance.BombenCommands
                    .FirstOrDefault(b => b.Command.Equals(command, System.StringComparison.OrdinalIgnoreCase));

                if (bombCmd != null)
                {
                    // Wenn der konfigurierten Nachricht leer ist, entferne alternativ das führende "!".
                    string bombText = bombCmd.Message;
                    if (string.IsNullOrEmpty(bombText))
                    {
                        bombText = command.TrimStart('!');
                    }
                    // Übergibt den konfigurierten Text an die DummyBomb.
                    DummyBomb.Senders.Enqueue(bombText);
                }
                // Optional: Falls kein passender BombCommand gefunden wird, könnte man hier einen Fallback einbauen.
            }
        }
    }
}
