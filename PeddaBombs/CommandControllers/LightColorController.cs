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
    public class LightColorController : MonoBehaviour, ICommandable
    {
        // Wird beim Start des Objekts aufgerufen.
        private void Start()
        {
            // Aktiviert den ColorPatch nur, wenn weder der Noodle- noch der Chroma-Modus aktiv sind.
            GetNormalColorPatch.Enable = !this._util.IsNoodle && !this._util.IsChroma;
        }

        // Gibt den Command-Key zurück, unter dem dieser Controller registriert ist.
        public string Key => CommandKey.LIGHT_COLOR;

        // Zeigt an, ob der TwitchFX-Plugin installiert ist.
        public bool IsInstallTwitchFX { get; set; }

        // Wird aufgerufen, wenn ein Chat-Command empfangen wird.
        public void Execute(MultiplexedPlatformService service, MultiplexedMessage message)
        {
            // Prüfen, ob das Ändern der Plattformfarben in der Konfiguration aktiviert ist.
            if (PluginConfig.Instance.IsPratformColorEnable != true) {
                return;
            }
            // Falls TwitchFX installiert ist, wird dieser Command nicht ausgeführt.
            if (this.IsInstallTwitchFX) {
                return;
            }
            // Aufteilen der eingehenden Nachricht anhand von Leerzeichen.
            var prams = message.Message.Split(' ');
            // Erwartet werden genau 3 Teile: der Command und zwei Parameter (z. B. Farben).
            if (prams.Length != 3) {
                return;
            }
            // Extrahiert den linken und rechten Farbparameter.
            var leftColor = prams[1];
            var rightColor = prams[2];
            // Wenn die Farbe im Dictionary vorhanden ist, wird sie gesetzt.
            if (ColorUtil.Colors.TryGetValue(leftColor, out var color0)) {
                GetNormalColorPatch.LeftColor = color0;
            }
            if (ColorUtil.Colors.TryGetValue(rightColor, out var color1)) {
                GetNormalColorPatch.RightColor = color1;
            }
        }

        // Private Hilfsvariable für Mod-bezogene Prüfungen.
        private BeatmapUtil _util;

        // Zenject-Injektion: Konstruktor-Methode zur Initialisierung benötigter Abhängigkeiten.
        [Inject]
        public void Constractor(ColorScheme scheme, BeatmapUtil util)
        {
            // Setzt die TwitchFX-Installation anhand des Plugin-Managers.
            this.IsInstallTwitchFX = PluginManager.GetPluginFromId("TwitchFX") != null;
            this._util = util;
            // Setzt initiale Farben aus dem ColorScheme.
            GetNormalColorPatch.RightColor = scheme.environmentColor0;
            GetNormalColorPatch.LeftColor = scheme.environmentColor1;
        }
    }
}
