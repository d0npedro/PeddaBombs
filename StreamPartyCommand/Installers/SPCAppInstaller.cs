using StreamPartyCommand.Models;
using StreamPartyCommand.Utilities;
using Zenject;

namespace StreamPartyCommand.Installers
{
    public class SPCAppInstaller : Installer
    {
        public override void InstallBindings()
        {
            _ = this.Container.BindInterfacesAndSelfTo<ChatCoreWrapper>().AsSingle().NonLazy();
            _ = this.Container.BindInterfacesAndSelfTo<CustomNoteUtil>().AsSingle().NonLazy();
            _ = this.Container.BindInterfacesAndSelfTo<FontAssetReader>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            _ = this.Container.BindInterfacesAndSelfTo<ParticleAssetLoader>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        }
    }
}
