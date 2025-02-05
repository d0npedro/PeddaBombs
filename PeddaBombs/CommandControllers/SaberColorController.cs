using CatCore.Services.Multiplexer;
using IPA.Loader;
using SiraUtil.Sabers;
using PeddaBombs.Configuration;
using PeddaBombs.Interfaces;
using PeddaBombs.Models;
using PeddaBombs.Utilities;
using UnityEngine;
using Zenject;
using PeddaBombs.Statics;

namespace PeddaBombs.CommandControllers {
    public class SaberColorController : MonoBehaviour, ICommandable {
        public bool IsInstallTwitchFX { get; set; }
        public string Key => CommandKey.SABER_COLOR;

        public bool RainbowLeft { get; private set; }
        public bool RainbowRight { get; private set; }

        private SaberModelManager _saberModelManager;

        private void Start() {
            this.enable = !this._util.IsNoodle && !this._util.IsChroma;
            this._rainbow = new Color[s_colorCount];
            var tmp = 1f / s_colorCount;
            for (var i = 0; i < s_colorCount; i++) {
                var hue = tmp * i;
                this._rainbow[i] = Color.HSVToRGB(hue, 1f, 1f);
            }
        }
        public void Execute(MultiplexedPlatformService service, MultiplexedMessage message) {
            if (!this.enable) {
                return;
            }
            if (PluginConfig.Instance.IsSaberColorEnable != true) {
                return;
            }
            if (this.IsInstallTwitchFX) {
                return;
            }
            var prams = message.Message.Split(' ');
            if (prams.Length != 3) {
                return;
            }
            var leftColor = prams[1];
            var rightColor = prams[2];
            if (ColorUtil.IsRainbow(leftColor)) {
                this.RainbowLeft = true;
            }
            if (ColorUtil.IsRainbow(rightColor)) {
                this.RainbowRight = true;
            }
            if (ColorUtil.Colors.TryGetValue(leftColor, out var color0)) {
                this._saberModelManager.SetColor(this._saberManager.leftSaber, color0);
                this.RainbowLeft = false;
            }
            if (ColorUtil.Colors.TryGetValue(rightColor, out var color1)) {
                this._saberModelManager.SetColor(this._saberManager.rightSaber, color1);
                this.RainbowRight = false;
            }
        }

        public void Update() {
            if (this.RainbowLeft) {
                this._saberModelManager.SetColor(this._saberManager.leftSaber, this._rainbow[this._leftColorIndex]);
            }
            if (this.RainbowRight) {
                this._saberModelManager.SetColor(this._saberManager.rightSaber, this._rainbow[this._rightColorIndex]);
            }
        }

        public void FixedUpdate() {
            this._leftColorIndex = Time.frameCount % s_colorCount;
            this._rightColorIndex = (Time.frameCount + (s_colorCount / 2)) % s_colorCount;
        }

        private BeatmapUtil _util;
        private Color[] _rainbow;
        private int _leftColorIndex;
        private int _rightColorIndex;
        public const int s_colorCount = 256;


        [Inject]
        public void Constractor(SaberManager saberManager, BeatmapUtil util, SaberModelManager modelManager) {
            this.IsInstallTwitchFX = PluginManager.GetPluginFromId("TwitchFX") != null;
            this._util = util;
            this._saberManager = saberManager;
            this._saberModelManager = modelManager;
        }
        private bool enable;
        private SaberManager _saberManager;
    }
}
