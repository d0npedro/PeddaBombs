using AssetBundleLoadingTools.Utilities;
using SiraUtil.Zenject;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PeddaBombs.Models {
    public class ParticleAssetLoader : MonoBehaviour, IAsyncInitializable {
        private static readonly string FontAssetPath = Path.Combine(Environment.CurrentDirectory, "UserData", "PBParticleAssets");
        public bool IsInitialized { get; private set; } = false;
        public ParticleSystem Particle { get; private set; } = null;

        public void OnDestroy() {
            if (this.Particle != null) {
                Destroy(this.Particle);
            }
        }

        public async Task LoadParticle() {
            this.IsInitialized = false;

            if (this.Particle != null) {
                Destroy(this.Particle);
            }
            if (!Directory.Exists(FontAssetPath)) {
                _ = Directory.CreateDirectory(FontAssetPath);
            }
            AssetBundle bundle = null;
            foreach (var filename in Directory.EnumerateFiles(FontAssetPath, "*.particle", SearchOption.TopDirectoryOnly)) {
                bundle = await AssetBundleExtensions.LoadFromFileAsync(filename);
                if (bundle != null) {
                    Plugin.Log.Info($"Loaded particle:{bundle}");
                    break;
                }
            }
            if (bundle != null) {
                foreach (var bundleItem in bundle.GetAllAssetNames()) {
                    var asset = await AssetBundleExtensions.LoadAssetAsync<GameObject>(bundle, Path.GetFileNameWithoutExtension(bundleItem));
                    if (asset != null) {
                        _ = ShaderRepair.FixShadersOnGameObject(asset);
                        this.Particle = asset.GetComponent<ParticleSystem>();
                        this.Particle.Stop();
                        break;
                    }
                }
                bundle.Unload(false);
            }
            this.IsInitialized = true;
        }

        public async Task InitializeAsync(CancellationToken token) {
            await this.LoadParticle();
        }
    }
}
