using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using PeddaBombs.Configuration;
using Zenject;

namespace PeddaBombs.Views
{
    internal class SettingViewController : BSMLAutomaticViewController, IInitializable
    {

        public string ResourceName => string.Join(".", this.GetType().Namespace, this.GetType().Name);
        [UIValue("is-bomb-enable")]
        public virtual bool IsBombEnable
        {
            get => PluginConfig.Instance.IsBombEnable;
            set => PluginConfig.Instance.IsBombEnable = value;
        }
        [UIValue("is-sabercolor-enable")]
        public virtual bool IsSaberColorEnable
        {
            get => PluginConfig.Instance.IsSaberColorEnable;
            set => PluginConfig.Instance.IsSaberColorEnable = value;
        }
        [UIValue("is-wallcolor-enable")]
        public virtual bool IsWallColorEnable
        {
            get => PluginConfig.Instance.IsWallColorEnable;
            set => PluginConfig.Instance.IsWallColorEnable = value;
        }
        [UIValue("is-notecolor-enable")]
        public virtual bool IsNoteColorEnable
        {
            get => PluginConfig.Instance.IsNoteColorEnable;
            set => PluginConfig.Instance.IsNoteColorEnable = value;
        }
        [UIValue("is-pratformcolor-enable")]
        public virtual bool IsPratformColorEnable
        {
            get => PluginConfig.Instance.IsPratformColorEnable;
            set => PluginConfig.Instance.IsPratformColorEnable = value;
        }

        public void Initialize()
        {
            BSMLSettings.Instance.AddSettingsMenu("<size=80%>PeddaBombs</size>", this.ResourceName, this);
        }

    }
}
