using HarmonyLib; // Für das Patchen von Methoden zur Laufzeit
using IPA; // Hauptnamespace des IPA-Frameworks
using IPA.Config; // Konfigurationsunterstützung von IPA
using IPA.Config.Stores; // Speicherung und Verwaltung der Konfiguration
using IPA.Loader; // Laden von Plugins über IPA
using SiraUtil.Zenject; // Zenject-Erweiterungen für Beat Saber
using PeddaBombs.Installers; // Enthält die Installer-Klassen für Zenject
using System.Reflection; // Für Reflection-Funktionen, z.B. PatchAll
using IPALogger = IPA.Logging.Logger; // Alias für den Logger von IPA

namespace PeddaBombs
{
    // Das Plugin-Attribut legt fest, dass es sich um ein Plugin handelt und wie es initialisiert wird.
    // RuntimeOptions.DynamicInit bedeutet, dass die Initialisierung dynamisch erfolgt.
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        // Statische Instanz, um auf das Plugin aus anderen Klassen zugreifen zu können.
        internal static Plugin Instance { get; private set; }

        // Logger, der für Debug- und Informationsausgaben genutzt wird.
        internal static IPALogger Log { get; private set; }

        // Eine Konstante, die die Harmony-ID definiert – wichtig für das Patchen von Methoden.
        public const string HARMONY_ID = "PeddaBombs.d0npedro.com.github";

        // Instanz von Harmony, die für das Patchen und späteres Unpatchen verwendet wird.
        private Harmony harmony;

        // Die Init-Methode wird beim Laden des Plugins aufgerufen.
        // Sie erhält den Logger, die Konfiguration und den Zenjector.
        [Init]
        public void Init(IPALogger logger, Config conf, Zenjector zenjector)
        {
            // Setzt die statische Instanz und den Logger.
            Instance = this;
            Log = logger;
            Log.Info("PeddaBombs initialized.");

            // Erzeugt die Plugin-Konfigurationsinstanz aus der Config-Datei.
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");

            // Erstellt eine neue Harmony-Instanz mit der definierten ID.
            this.harmony = new Harmony(HARMONY_ID);

            // Installiert die verschiedenen Installer von Zenject:
            // - PBAppInstaller: Für App-weit gültige Bindings.
            // - PBGameInstaller: Für Bindings im Spiel (Player).
            // - PBMenuInstaller: Für Bindings im Menü.
            zenjector.Install<PBAppInstaller>(Location.App);
            zenjector.Install<PBGameInstaller>(Location.Player);
            zenjector.Install<PBMenuInstaller>(Location.Menu);
        }

        // OnApplicationStart wird aufgerufen, wenn die Anwendung startet.
        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");

            // Prüft, ob TwitchFX installiert ist, und loggt dies.
            var twitchFX = PluginManager.GetPluginFromId("TwitchFX");
            if (twitchFX != null)
            {
                Logger.Debug("TwitchFX is Installing.");
            }
        }

        // OnApplicationQuit wird aufgerufen, wenn die Anwendung beendet wird.
        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");
        }

        // OnEnable wird aufgerufen, wenn das Plugin aktiviert wird.
        [OnEnable]
        public void OnEnable()
        {
            // Patcht alle Methoden im aktuellen Assembly, die Harmony-Patches verwenden.
            this.harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        // OnDisable wird aufgerufen, wenn das Plugin deaktiviert wird.
        [OnDisable]
        public void OnDisable()
        {
            // Entfernt alle Patches, die von dieser Harmony-Instanz gesetzt wurden.
            this.harmony.UnpatchSelf();
        }
    }
}
