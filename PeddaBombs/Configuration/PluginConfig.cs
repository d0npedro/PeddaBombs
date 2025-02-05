using IPA.Config.Stores;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace PeddaBombs.Configuration
{
    // Hilfsklasse, die einen Bomben-Command repräsentiert.
    internal class BombCommand
    {
        // Der Chat-Befehl, z. B. "!rofl"
        public virtual string Command { get; set; }
        // Die Nachricht, die an die DummyBomb-Funktion übergeben wird, z. B. "ROFL !"
        public virtual string Message { get; set; }
    }

    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }

        public virtual bool IsBombEnable { get; set; } = true;
        public virtual float TextViewSec { get; set; } = 1f;
        public virtual float MissTextViewSec { get; set; } = 0.7f;
        public virtual bool IsSaberColorEnable { get; set; } = true;
        public virtual bool IsWallColorEnable { get; set; } = true;
        public virtual bool IsNoteColorEnable { get; set; } = true;
        public virtual bool IsPratformColorEnable { get; set; } = true;
        public virtual int NameObjectLayer { get; set; } = 0;
        public virtual bool ReloadeIfMissCut { get; set; } = true;

        // Privates Backing-Field, das die Default-Werte enthält.
        private List<BombCommand> _bombenCommands = new List<BombCommand>
        {
            new BombCommand { Command = "!rofl", Message = "ROFL !" },
            new BombCommand { Command = "!awesome", Message = "You are awesome" },
            new BombCommand { Command = "!usw", Message = "Und so Weiter" }
        };

        // Die BombenCommands-Eigenschaft verwendet das Backing-Field.
        public virtual List<BombCommand> BombenCommands
        {
            get => _bombenCommands;
            set => _bombenCommands = value;
        }

        /// <summary>
        /// Diese Methode wird aufgerufen, wenn BSIPA die Config von der Festplatte liest
        /// (auch, wenn Dateiänderungen erkannt werden).
        /// </summary>
        public virtual void OnReload()
        {
            // Falls aus irgendeinem Grund BombenCommands leer oder null ist, setzen wir die Default-Werte.
            if (BombenCommands == null || BombenCommands.Count == 0) {
                BombenCommands = new List<BombCommand>
                {
                    new BombCommand { Command = "!rofl", Message = "ROFL !" },
                    new BombCommand { Command = "!awesome", Message = "You are awesome" },
                    new BombCommand { Command = "!usw", Message = "Und so Weiter" }
                };
            }
        }

        /// <summary>
        /// Diese Methode wird verwendet, um BSIPA mitzuteilen, dass die Config aktualisiert werden soll.
        /// Sie wird auch aufgerufen, wenn BSIPA feststellt, dass die Datei geändert wurde.
        /// </summary>
        public virtual void Changed()
        {
            // Hier kannst du Code einfügen, der ausgeführt wird, wenn sich die Config ändert.
        }

        /// <summary>
        /// Mit dieser Methode werden die Werte von einer anderen Config-Instanz in diese kopiert.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            this.IsBombEnable = other.IsBombEnable;
            this.TextViewSec = other.TextViewSec;
            this.MissTextViewSec = other.MissTextViewSec;
            this.IsSaberColorEnable = other.IsSaberColorEnable;
            this.IsWallColorEnable = other.IsWallColorEnable;
            this.IsNoteColorEnable = other.IsNoteColorEnable;
            this.IsPratformColorEnable = other.IsPratformColorEnable;
            this.NameObjectLayer = other.NameObjectLayer;
            this.ReloadeIfMissCut = other.ReloadeIfMissCut;
            this.BombenCommands = other.BombenCommands;
        }
    }
}
