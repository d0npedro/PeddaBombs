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

namespace PeddaBombs.CommandControllers {
    public class LightColorController : MonoBehaviour, ICommandable {
        private void Start() {
            GetNormalColorPatch.Enable = !this._util.IsNoodle && !this._util.IsChroma;
        }

        public string Key => CommandKey.LIGHT_COLOR;

        public bool IsInstallTwitchFX { get; set; }

        public void Execute(MultiplexedPlatformService service, MultiplexedMessage message) {
            if (PluginConfig.Instance.IsPratformColorEnable != true) {
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
            if (ColorUtil.Colors.TryGetValue(leftColor, out var color0)) {
                GetNormalColorPatch.LeftColor = color0;
            }
            if (ColorUtil.Colors.TryGetValue(rightColor, out var color1)) {
                GetNormalColorPatch.RightColor = color1;
            }
        }
        private BeatmapUtil _util;
        [Inject]
        public void Constractor(ColorScheme scheme, BeatmapUtil util) {
            this.IsInstallTwitchFX = PluginManager.GetPluginFromId("TwitchFX") != null;
            this._util = util;
            GetNormalColorPatch.RightColor = scheme.environmentColor0;
            GetNormalColorPatch.LeftColor = scheme.environmentColor1;
        }
    }
}
