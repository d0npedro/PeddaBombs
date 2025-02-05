using CatCore.Services.Multiplexer;
using IPA.Loader;
using PeddaBombs.Configuration;
using PeddaBombs.HarmonyPathches;
using PeddaBombs.Interfaces;
using PeddaBombs.Models;
using PeddaBombs.Statics;
using PeddaBombs.Utilities;
using UnityEngine;
using Zenject;

namespace PeddaBombs.CommandControllers
{
    public class WallColorController : MonoBehaviour, ICommandable
    {
        // Flag, ob TwitchFX installiert ist.
        public bool IsInstallTwitchFX { get; set; }
        // Gibt den registrierten Command-Key zurück.
        public string Key => CommandKey.WALL_COLOR;

        // Wird beim Start des Objekts aufgerufen.
        private void Start()
        {
            // Aktiviert den Patch nur, wenn weder Noodle noch Chroma aktiv sind.
            StretchableObstaclePatch.Enable = !this._util.IsNoodle && !this._util.IsChroma;
        }

        // Wird aufgerufen, wenn der Command empfangen wird.
        public void Execute(MultiplexedPlatformService service, MultiplexedMessage message)
        {
            // Prüft, ob die Wandfarbe in der Konfiguration aktiviert ist.
            if (PluginConfig.Instance.IsWallColorEnable != true) {
                return;
            }
            // Beendet die Ausführung, wenn TwitchFX installiert ist.
            if (this.IsInstallTwitchFX) {
                return;
            }
            // Teilt die eingehende Nachricht in Parameter.
            var messageArray = message.Message.Split(' ');
            // Erwartet werden 2 Teile: Command und ein Parameter (Farbe).
            if (messageArray.Length != 2) {
                return;
            }

            // Wenn der Parameter den "Rainbow"-Modus anfordert, wird das Flag gesetzt.
            if (ColorUtil.IsRainbow(messageArray[1])) {
                this._rainbow = true;
            }
            // Falls ein fester Farbwert angegeben wurde, wird dieser gesetzt und der Regenbogen-Effekt deaktiviert.
            if (ColorUtil.Colors.TryGetValue(messageArray[1], out var color)) {
                StretchableObstaclePatch.WallColor = color;
                this._rainbow = false;
            }
        }

        // Update wird jeden Frame aufgerufen.
        private void Update()
        {
            // Falls der Regenbogen-Effekt aktiv ist, wird die Wandfarbe dynamisch aktualisiert.
            if (this._rainbow) {
                this._rainbowUtil.SetWallRainbowColor(this._rainbowUtil.WallRainbowColor());
            }
        }

        // Private Hilfsvariablen.
        private BeatmapUtil _util;
        private RainbowUtil _rainbowUtil;
        private bool _rainbow = false;

        // Zenject-Injektion: Initialisiert Abhängigkeiten.
        [Inject]
        public void Constractor(ColorManager manager, BeatmapUtil util, RainbowUtil rainbowUtil)
        {
            // Prüft, ob TwitchFX installiert ist.
            this.IsInstallTwitchFX = PluginManager.GetPluginFromId("TwitchFX") != null;
            this._util = util;
            this._rainbowUtil = rainbowUtil;
            this._rainbow = false;
            // Setzt die Standardwandfarbe anhand des ColorManagers.
            StretchableObstaclePatch.WallColor = manager.obstaclesColor;
        }
    }
}
