﻿using ChatCore.Interfaces;
using StreamPartyCommand.Configuration;
using StreamPartyCommand.HarmonyPathches;
using StreamPartyCommand.Interfaces;
using StreamPartyCommand.Models;
using StreamPartyCommand.Staics;
using StreamPartyCommand.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace StreamPartyCommand.CommandControllers
{
    public class NoteColorController : MonoBehaviour, ICommandable
    {
        private void Start()
        {
            ColorManagerColorForTypePatch.Enable = !this._util.IsChroma;
        }
        public string Key => CommandKey.NOTE_COLOR;

        public void Execute(IChatService service, IChatMessage message)
        {
            if (PluginConfig.Instance.IsNoteColorEnable != true) {
                return;
            }
            var prams = message.Message.Split(' ');
            if (prams.Length != 3) {
                return;
            }
            var leftColor = prams[1];
            var rightColor = prams[2];
            if (ColorUtil.Colors.TryGetValue(leftColor, out var color0)) {
                ColorManagerColorForTypePatch.LeftColor = color0;
            }
            if (ColorUtil.Colors.TryGetValue(rightColor, out var color1)) {
                ColorManagerColorForTypePatch.RightColor = color1;
            }
        }
        private BeatmapUtil _util;
        [Inject]
        public void Constractor(ColorScheme scheme, BeatmapUtil util)
        {
            this._util = util;
            ColorManagerColorForTypePatch.LeftColor = scheme.saberAColor;
            ColorManagerColorForTypePatch.RightColor = scheme.saberBColor;
        }
	}
}
