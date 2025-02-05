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
    public class NoteColorController : MonoBehaviour, ICommandable
    {
        // Wird beim Start des Objekts aufgerufen.
        private void Start()
        {
            // Aktiviert den Patch nur, wenn weder Noodle noch Chroma aktiv sind.
            ColorManagerColorForTypePatch.Enable = !this._util.IsNoodle && !this._util.IsChroma;
            // Initialisiert das Array für die Farbabstufungen (Rainbow-Effekt).
            this.Colors = new Color[s_colorCount];
            var tmp = 1f / s_colorCount;
            for (var i = 0; i < s_colorCount; i++) {
                // Berechnet den Farbton und wandelt ihn in einen RGB-Farbwert um.
                var hue = tmp * i;
                this.Colors[i] = Color.HSVToRGB(hue, 1f, 1f);
            }
        }

        // Gibt den Command-Key zurück, unter dem dieser Controller registriert ist.
        public string Key => CommandKey.NOTE_COLOR;

        // Flag, ob TwitchFX installiert ist.
        public bool IsInstallTwitchFX { get; set; }
        // Array, das die erzeugte Regenbogen-Farbpalette enthält.
        public Color[] Colors { get; private set; }
        // Flags für Regenbogeneffekt auf linker bzw. rechter Seite.
        public bool RainbowLeft { get; private set; }
        public bool RainbowRight { get; private set; }
        // Indizes für die Auswahl der Farbe aus dem Colors-Array.
        public int LeftColorIndex { get; private set; }
        public int RightColorIndex { get; private set; }

        // Wird aufgerufen, wenn der entsprechende Chat-Command empfangen wird.
        public void Execute(MultiplexedPlatformService service, MultiplexedMessage message)
        {
            // Prüft, ob NoteColor-Funktionalität in der Konfiguration aktiviert ist.
            if (PluginConfig.Instance.IsNoteColorEnable != true) {
                return;
            }
            // Beendet die Ausführung, wenn TwitchFX installiert ist.
            if (this.IsInstallTwitchFX) {
                return;
            }
            // Teilt die Nachricht in einzelne Parameter.
            var prams = message.Message.Split(' ');
            // Erwartet werden exakt 3 Teile (Command + 2 Parameter).
            if (prams.Length != 3) {
                return;
            }
            // Extrahiert die Farbparameter für links und rechts.
            var leftColor = prams[1];
            var rightColor = prams[2];

            // Überprüft, ob der Parameter für links den "Rainbow"-Modus anfordert.
            if (ColorUtil.IsRainbow(leftColor)) {
                this.RainbowLeft = true;
            }
            // Analog für den rechten Parameter.
            if (ColorUtil.IsRainbow(rightColor)) {
                this.RainbowRight = true;
            }

            // Wenn ein fester Farbwert gefunden wird, wird dieser gesetzt und der Regenbogeneffekt deaktiviert.
            if (ColorUtil.Colors.TryGetValue(leftColor, out var color0)) {
                ColorManagerColorForTypePatch.LeftColor = color0;
                this.RainbowLeft = false;
            }

            if (ColorUtil.Colors.TryGetValue(rightColor, out var color1)) {
                ColorManagerColorForTypePatch.RightColor = color1;
                this.RainbowRight = false;
            }
        }

        // FixedUpdate wird in einem festen Zeitintervall aufgerufen.
        public void FixedUpdate()
        {
            // Berechnet die aktuellen Indizes für den Regenbogen-Effekt basierend auf der Frame-Anzahl.
            this.LeftColorIndex = Time.frameCount % s_colorCount;
            this.RightColorIndex = (Time.frameCount + (s_colorCount / 2)) % s_colorCount;
        }

        // Private Hilfsvariable für den Zugriff auf Mod-spezifische Einstellungen.
        private BeatmapUtil _util;
        // Konstante für die Anzahl der Farbwerte im Regenbogen.
        public const int s_colorCount = 256;

        // Zenject-Injektion: Konstruktor-Methode zur Initialisierung benötigter Abhängigkeiten.
        [Inject]
        public void Constractor(ColorScheme scheme, BeatmapUtil util)
        {
            this.IsInstallTwitchFX = PluginManager.GetPluginFromId("TwitchFX") != null;
            this._util = util;
            // Setzt Standardfarben aus dem ColorScheme.
            ColorManagerColorForTypePatch.LeftColor = scheme.saberAColor;
            ColorManagerColorForTypePatch.RightColor = scheme.saberBColor;
        }
    }
}
