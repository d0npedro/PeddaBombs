using AssetBundleLoadingTools.Utilities;
using BeatSaberMarkupLanguage;
using SiraUtil.Zenject;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace PeddaBombs.Models
{
    public class FontAssetReader : MonoBehaviour, IAsyncInitializable
    {
        private static readonly string FontAssetPath = Path.Combine(Environment.CurrentDirectory, "UserData", "PBFontAssets");
        private static readonly string MainFontPath = Path.Combine(FontAssetPath, "Main");

        private static Shader _tmpNoGlowFontShader;
        public static Shader TMPNoGlowFontShader => _tmpNoGlowFontShader ?? (_tmpNoGlowFontShader = BeatSaberUI.MainTextFont?.material.shader);

        public bool IsInitialized { get; private set; } = false;

        private TMP_FontAsset _mainFont = null;

        public TMP_FontAsset MainFont
        {
            get
            {
                if (!this._mainFont) {
                    return null;
                }
                if (this._mainFont.material.shader != TMPNoGlowFontShader) {
                    this._mainFont.material.shader = TMPNoGlowFontShader;
                }
                return this._mainFont;
            }
            private set => this._mainFont = value;
        }

        public async Task CreateChatFont()
        {
            this.IsInitialized = false;
            while (TMPNoGlowFontShader == null) {
                await Task.Yield();
            }
            if (this.MainFont != null) {
                Destroy(this.MainFont);
            }
            if (!Directory.Exists(MainFontPath)) {
                _ = Directory.CreateDirectory(MainFontPath);
            }

            AssetBundle bundle = null;
            foreach (var filename in Directory.EnumerateFiles(MainFontPath, "*.assets", SearchOption.TopDirectoryOnly)) {
                bundle = await AssetBundleExtensions.LoadFromFileAsync(filename);
                if (bundle != null) {
                    break;
                }
            }
            if (bundle != null) {
                foreach (var bundleItem in bundle.GetAllAssetNames()) {
                    var asset = await AssetBundleExtensions.LoadAssetAsync<TMP_FontAsset>(bundle, bundleItem);
                    if (asset != null) {
                        this.MainFont = asset;
                        bundle.Unload(false);
                        break;
                    }
                }
            }

            this.IsInitialized = true;
        }

        public async Task InitializeAsync(CancellationToken token)
        {
            await this.CreateChatFont();
        }
    }
}
