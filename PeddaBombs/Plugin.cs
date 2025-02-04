﻿using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using SiraUtil.Zenject;
using PeddaBombs.Installers;
using System.Reflection;
using IPALogger = IPA.Logging.Logger;

namespace PeddaBombs
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        public const string HARMONY_ID = "PeddaBombs.d0npedro.com.github";
        private Harmony harmony;

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger, Config conf, Zenjector zenjector)
        {
            Instance = this;
            Log = logger;
            Log.Info("PeddaBombs initialized.");
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
            this.harmony = new Harmony(HARMONY_ID);
            zenjector.Install<PBAppInstaller>(Location.App);
            zenjector.Install<PBGameInstaller>(Location.Player);
            zenjector.Install<PBMenuInstaller>(Location.Menu);
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");
            var twitchFX = PluginManager.GetPluginFromId("TwitchFX");
            if (twitchFX != null) {
                Logger.Debug("TwitchFX is Installing.");
            }
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");
        }

        [OnEnable]
        public void OnEnable()
        {
            this.harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void OnDisable()
        {
            this.harmony.UnpatchSelf();
        }
    }
}
