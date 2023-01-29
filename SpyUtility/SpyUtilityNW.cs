using System;
using HarmonyLib;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using Respawning;
using SpyUtilityNW.Events;

namespace SpyUtilityNW
{
    public class SpyUtilityNW
    {
        [PluginConfig] public Config Config;

        private Harmony harmony;

        /// <summary>
        ///     Gets a static instance of the <see cref="SpyUtilityNW" /> class.
        /// </summary>
        public static SpyUtilityNW Instance { get; private set; }

        public SpyManager SpyManager { get; set; }

        [PluginEntryPoint("SpyUtilityNW", "1.0.8",
            "Spy plugin", "Undid Iridium")]
        private void LoadPlugin()
        {
            Instance = this;
            if (Config.IsEnabled)
            {
                harmony = new Harmony($"com.Undid-Iridium.SpyUtilityNW.{DateTime.UtcNow.Ticks}");
                harmony.PatchAll();

                PluginAPI.Events.EventManager.RegisterEvents(this);
                PluginAPI.Events.EventManager.RegisterEvents<SpyManager>(this);
                PatchedEventHandlers.BeforeTeamRespawn += SpyManager.OnWaveSpawn;
                PatchedEventHandlers.FlashbangProcessing += SpyManager.OnFlashBang;
                
                Log.Debug("We have started our plugin SpyUtilityNW!!", SpyUtilityNW.Instance.Config.Debug);
            }
        }
    }
}