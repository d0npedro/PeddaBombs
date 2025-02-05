using CatCore.Services.Multiplexer;
using IPA.Loader;
using SiraUtil.Sabers;
using PeddaBombs.Configuration;
using PeddaBombs.Interfaces;
using PeddaBombs.Models;
using PeddaBombs.Statics;
using PeddaBombs.Utilities;
using UnityEngine;
using Zenject;

namespace PeddaBombs.CommandControllers
{
    public class SaberColorController : MonoBehaviour, ICommandable
    {
        // Flag, ob TwitchFX installiert ist.
        public bool IsInstallTwitchFX { get; set; }
        // Gibt den registrierten Command-Key zurück.
        public string Key => CommandKey.SABER_COLOR;

        // Flags, ob der Regenbogen-Effekt für die linke bzw. rechte Klinge aktiviert ist.
        public bool RainbowLeft { get; private set; }
        public bool RainbowRight { get; private set; }

        // Referenz zur Verwaltung der Sabermodelle (z.B. zum Setzen der Farben).
        private SaberModelManager _saberModelManager;

        // Wird beim Start des Objekts aufgerufen.
        private void Start()
        {
            // Aktiviert diesen Controller nur, wenn weder Noodle noch Chroma aktiv sind.
            this.enable = !this._util.IsNoodle && !this._util.IsChroma;
            // Initialisiert das Array für den Regenbogen-Effekt.
            this._rainbow = new Color[s_colorCount];
            var tmp = 1f / s_colorCount;
            for (var i = 0; i < s_colorCount; i++) {
                // Berechnet den Farbton und wandelt ihn in einen RGB-Farbwert um.
                var hue = tmp * i;
                this._rainbow[i] = Color.HSVToRGB(hue, 1f, 1f);
            }
        }

        // Wird aufgerufen, wenn der Command empfangen wird.
        public void Execute(MultiplexedPlatformService service, MultiplexedMessage message)
        {
            // Prüft, ob dieser Controller aktiviert ist.
            if (!this.enable) {
                return;
            }
            // Prüft, ob die Änderung der Sabers-Farbe in der Konfiguration aktiviert ist.
            if (PluginConfig.Instance.IsSaberColorEnable != true) {
                return;
            }
            // Falls TwitchFX installiert ist, wird die Ausführung abgebrochen.
            if (this.IsInstallTwitchFX) {
                return;
            }
            // Teilt die eingehende Nachricht in Parameter auf.
            var prams = message.Message.Split(' ');
            // Erwartet werden exakt 3 Teile: Command + 2 Parameter (Farben).
            if (prams.Length != 3) {
                return;
            }
            // Extrahiert die Parameter für linke und rechte Farbe.
            var leftColor = prams[1];
            var rightColor = prams[2];
            // Prüft, ob der linke Parameter den Regenbogen-Effekt anfordert.
            if (ColorUtil.IsRainbow(leftColor)) {
                this.RainbowLeft = true;
            }
            // Analog für den rechten Parameter.
            if (ColorUtil.IsRainbow(rightColor)) {
                this.RainbowRight = true;
            }
            // Falls eine konkrete Farbe angegeben wurde, wird diese gesetzt und der Regenbogeneffekt deaktiviert.
            if (ColorUtil.Colors.TryGetValue(leftColor, out var color0)) {
                this._saberModelManager.SetColor(this._saberManager.leftSaber, color0);
                this.RainbowLeft = false;
            }
            if (ColorUtil.Colors.TryGetValue(rightColor, out var color1)) {
                this._saberModelManager.SetColor(this._saberManager.rightSaber, color1);
                this.RainbowRight = false;
            }
        }

        // Update wird jeden Frame aufgerufen.
        public void Update()
        {
            // Wenn der Regenbogen-Effekt aktiv ist, wird die Farbe anhand des aktuellen Index aktualisiert.
            if (this.RainbowLeft) {
                this._saberModelManager.SetColor(this._saberManager.leftSaber, this._rainbow[this._leftColorIndex]);
            }
            if (this.RainbowRight) {
                this._saberModelManager.SetColor(this._saberManager.rightSaber, this._rainbow[this._rightColorIndex]);
            }
        }

        // FixedUpdate wird in regelmäßigen Abständen aufgerufen.
        public void FixedUpdate()
        {
            // Berechnet die aktuellen Indizes für den Regenbogen-Effekt basierend auf der Frame-Anzahl.
            this._leftColorIndex = Time.frameCount % s_colorCount;
            this._rightColorIndex = (Time.frameCount + (s_colorCount / 2)) % s_colorCount;
        }

        // Private Hilfsvariablen und Konstanten.
        private BeatmapUtil _util;
        private Color[] _rainbow;
        private int _leftColorIndex;
        private int _rightColorIndex;
        public const int s_colorCount = 256;
        private bool enable;
        private SaberManager _saberManager;

        // Zenject-Injektion: Initialisiert Abhängigkeiten.
        [Inject]
        public void Constractor(SaberManager saberManager, BeatmapUtil util, SaberModelManager modelManager)
        {
            // Prüft, ob TwitchFX installiert ist.
            this.IsInstallTwitchFX = PluginManager.GetPluginFromId("TwitchFX") != null;
            this._util = util;
            this._saberManager = saberManager;
            this._saberModelManager = modelManager;
        }
    }
}
