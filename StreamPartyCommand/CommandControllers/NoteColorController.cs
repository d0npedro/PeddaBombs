using CatCore.Services.Multiplexer;
using IPA.Loader;
using StreamPartyCommand.Configuration;
using StreamPartyCommand.HarmonyPathches;
using StreamPartyCommand.Interfaces;
using StreamPartyCommand.Models;
using StreamPartyCommand.Staics;
using StreamPartyCommand.Utilities;
using UnityEngine;
using Zenject;

namespace StreamPartyCommand.CommandControllers
{
    public class NoteColorController : MonoBehaviour, ICommandable
    {
        private void Start()
        {
            ColorManagerColorForTypePatch.Enable = !this._util.IsNoodle && !this._util.IsChroma;
            this.Colors = new Color[s_colorCount];
            var tmp = 1f / s_colorCount;
            for (var i = 0; i < s_colorCount; i++) {
                var hue = tmp * i;
                this.Colors[i] = Color.HSVToRGB(hue, 1f, 1f);
            }
        }

        public string Key => CommandKey.NOTE_COLOR;
        public bool IsInstallTwitchFX { get; set; }
        public Color[] Colors { get; private set; }
        public bool RainbowLeft { get; private set; }
        public bool RainbowRight { get; private set; }
        public int LeftColorIndex { get; private set; }
        public int RightColorIndex { get; private set; }

        public void Execute(MultiplexedPlatformService service, MultiplexedMessage message)
        {
            if (PluginConfig.Instance.IsNoteColorEnable != true) {
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
                ColorManagerColorForTypePatch.LeftColor = color0;
                this.RainbowLeft = false;
            }

            if (ColorUtil.Colors.TryGetValue(rightColor, out var color1)) {
                ColorManagerColorForTypePatch.RightColor = color1;
                this.RainbowRight = false;
            }
        }

        public void FixedUpdate()
        {
            this.LeftColorIndex = Time.frameCount % s_colorCount;
            this.RightColorIndex = (Time.frameCount + (s_colorCount / 2)) % s_colorCount;
        }

        private BeatmapUtil _util;
        public const int s_colorCount = 256;

        [Inject]
        public void Constractor(ColorScheme scheme, BeatmapUtil util)
        {
            this.IsInstallTwitchFX = PluginManager.GetPluginFromId("TwitchFX") != null;
            this._util = util;
            ColorManagerColorForTypePatch.LeftColor = scheme.saberAColor;
            ColorManagerColorForTypePatch.RightColor = scheme.saberBColor;
        }
    }
}
