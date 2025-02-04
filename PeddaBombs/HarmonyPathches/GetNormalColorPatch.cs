using HarmonyLib;
using System;
using UnityEngine;

namespace PeddaBombs.HarmonyPathches
{
    [HarmonyPatch(typeof(LightSwitchEventEffect), nameof(LightSwitchEventEffect.GetNormalColor), new Type[] { typeof(int), typeof(bool) })]
    public class GetNormalColorPatch
    {
        public static void Postfix(ref int beatmapEventValue, ref bool colorBoost, ref Color __result)
        {
            if (!Enable) {
                return;
            }
            switch (BeatmapEventDataLightsExtensions.GetLightColorTypeFromEventDataValue(beatmapEventValue)) {
                case EnvironmentColorType.Color0:
                    if (!colorBoost) {
                        __result = LeftColor;
                    }
                    break;
                case EnvironmentColorType.Color1:
                    if (!colorBoost) {
                        __result = RightColor;
                    }
                    break;
                case EnvironmentColorType.ColorW:
                    break;
                default:
                    __result = LeftColor;
                    break;
            }
        }
        internal static bool Enable { get; set; }
        internal static Color RightColor { get; set; }
        internal static Color LeftColor { get; set; }
    }
}
