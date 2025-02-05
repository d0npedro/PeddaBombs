using PeddaBombs.Models;
using PeddaBombs.Utilities;
using Zenject;

namespace PeddaBombs.Installers {
    public class PBAppInstaller : Installer {
        public override void InstallBindings() {
            _ = this.Container.BindInterfacesAndSelfTo<ChatCoreWrapper>().AsSingle().NonLazy();
            _ = this.Container.BindInterfacesAndSelfTo<CustomNoteUtil>().AsSingle().NonLazy();
            _ = this.Container.BindInterfacesAndSelfTo<FontAssetReader>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            _ = this.Container.BindInterfacesAndSelfTo<ParticleAssetLoader>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        }
    }
}
