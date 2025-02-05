using PeddaBombs.Views;
using Zenject;

namespace PeddaBombs.Installers {
    public class PBMenuInstaller : MonoInstaller {
        public override void InstallBindings() {
            _ = this.Container.BindInterfacesAndSelfTo<SettingViewController>().FromNewComponentAsViewController().AsSingle().NonLazy();
        }
    }
}
