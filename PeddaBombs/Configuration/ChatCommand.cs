using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;

namespace PeddaBombs.Configuration
{
    public class ChatCommand
    {
        public string CommandKey { get; set; }      // Der Schlüssel des Befehls (z.B. "!bomb")
        public string CommandType { get; set; }    // Der Typ des Befehls (z.B. "bomb", "noteColor")
        public int PermissionLevel { get; set; }   // Berechtigungsstufe (z.B. 0 = alle, 1 = Mods, 2 = Admins)
        public string Message { get; set; }        // Optionale Nachricht oder spezifischer Output

        public ChatCommand(string key, string type, int level, string message = "")
        {
            CommandKey = key;
            CommandType = type;
            PermissionLevel = level;
            Message = message;
        }
    }

    public class CommandConfig
    {
        private static readonly string FilePath = "UserData/PeddaBombs/commands.json";

        public List<ChatCommand> Commands { get; set; } = new List<ChatCommand>();

        public void Load()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                Commands = JsonConvert.DeserializeObject<List<ChatCommand>>(json) ?? new List<ChatCommand>();
            }
            else
            {
                Save(); // Falls Datei nicht existiert, mit Standardwerten speichern
            }
        }

        public void Save()
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(Commands, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }


        public void AddCommand(ChatCommand command)
        {
            if (!Commands.Exists(c => c.CommandKey == command.CommandKey))
            {
                Commands.Add(command);
                Save();
            }
        }

        public ChatCommand GetCommand(string key)
        {
            return Commands.Find(c => c.CommandKey == key);
        }
    }
}
