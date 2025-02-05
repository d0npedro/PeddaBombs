using BeatSaberMarkupLanguage.Attributes;      // Ermöglicht das Verwenden von Attributen wie [UIValue] für die Bindung von UI-Elementen an Properties.
using BeatSaberMarkupLanguage.Settings;        // Stellt Funktionen bereit, um Einstellungsmenüs in BSML zu registrieren.
using BeatSaberMarkupLanguage.ViewControllers; // Enthält Basis-Klassen für ViewController, die mit BSML arbeiten.
using PeddaBombs.Configuration;                // Zugriff auf die Plugin-Konfiguration (PluginConfig), in der die Einstellungen gespeichert sind.
using Zenject;                                 // Dependency Injection Framework, das hier zur Initialisierung genutzt wird.

namespace PeddaBombs.Views
{
    // Der SettingViewController erbt von BSMLAutomaticViewController, was eine automatische Bindung
    // der UI-Elemente an die Properties ermöglicht. Durch die Implementierung von IInitializable
    // wird sichergestellt, dass die Initialize()-Methode beim Start des Plugins aufgerufen wird.
    internal class SettingViewController : BSMLAutomaticViewController, IInitializable
    {
        // ResourceName wird dynamisch aus dem Namespace und dem Klassennamen erzeugt.
        // Damit wird der Pfad zur entsprechenden BSML-Ressourcendatei bestimmt, die das Layout definiert.
        public string ResourceName => string.Join(".", this.GetType().Namespace, this.GetType().Name);

        // Mit [UIValue("is-bomb-enable")] wird diese Property an ein UI-Element gebunden,
        // das den Schlüssel "is-bomb-enable" verwendet. Änderungen in der UI werden automatisch
        // in die Plugin-Konfiguration (PluginConfig.Instance) übernommen.
        [UIValue("is-bomb-enable")]
        public virtual bool IsBombEnable
        {
            get => PluginConfig.Instance.IsBombEnable; // Gibt den aktuellen Wert aus der Konfiguration zurück.
            set => PluginConfig.Instance.IsBombEnable = value; // Aktualisiert den Konfigurationswert, wenn der Benutzer die Einstellung ändert.
        }

        // Bindet die Einstellung "is-sabercolor-enable" an ein UI-Element.
        [UIValue("is-sabercolor-enable")]
        public virtual bool IsSaberColorEnable
        {
            get => PluginConfig.Instance.IsSaberColorEnable;
            set => PluginConfig.Instance.IsSaberColorEnable = value;
        }

        // Bindet die Einstellung "is-wallcolor-enable" an ein UI-Element.
        [UIValue("is-wallcolor-enable")]
        public virtual bool IsWallColorEnable
        {
            get => PluginConfig.Instance.IsWallColorEnable;
            set => PluginConfig.Instance.IsWallColorEnable = value;
        }

        // Bindet die Einstellung "is-notecolor-enable" an ein UI-Element.
        [UIValue("is-notecolor-enable")]
        public virtual bool IsNoteColorEnable
        {
            get => PluginConfig.Instance.IsNoteColorEnable;
            set => PluginConfig.Instance.IsNoteColorEnable = value;
        }

        // Bindet die Einstellung "is-pratformcolor-enable" an ein UI-Element.
        [UIValue("is-pratformcolor-enable")]
        public virtual bool IsPratformColorEnable
        {
            get => PluginConfig.Instance.IsPratformColorEnable;
            set => PluginConfig.Instance.IsPratformColorEnable = value;
        }

        // Die Initialize()-Methode wird von Zenject beim Start aufgerufen (durch das IInitializable-Interface).
        // Hier wird das Einstellungsmenü in die BSML-Settings-Oberfläche integriert.
        public void Initialize()
        {
            // BSMLSettings.Instance.AddSettingsMenu fügt ein neues Einstellungsmenü hinzu:
            // - Der erste Parameter ist der Titel, hier mit HTML-ähnlicher Formatierung.
            // - Der zweite Parameter (ResourceName) gibt an, welche BSML-Ressourcendatei (Layout) geladen werden soll.
            // - Der dritte Parameter ist dieser ViewController, der die Logik und Bindungen bereitstellt.
            BSMLSettings.Instance.AddSettingsMenu("<size=80%>PeddaBombs</size>", this.ResourceName, this);
        }
    }
}
