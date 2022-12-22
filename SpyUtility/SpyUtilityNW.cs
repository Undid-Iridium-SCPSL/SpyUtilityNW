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

        [PluginEntryPoint("SpyUtilityNW", "1.0.0",
            "CI Spy plugin", "Undid Iridium")]
        private void LoadPlugin()
        {
            Instance = this;
            if (Config.IsEnabled)
            {
                harmony = new Harmony($"com.Undid-Iridium.SpyUtilityNW.{DateTime.UtcNow.Ticks}");
                harmony.PatchAll();

                // SpyManager = new CISpyManager();
                PluginAPI.Events.EventManager.RegisterEvents(this);
                PluginAPI.Events.EventManager.RegisterEvents<SpyManager>(this);
                PatchedEventHandlers.BeforeTeamRespawn += SpyManager.OnWaveSpawn;
                

                Log.Debug("We have started our plugin SpyUtilityNW!!", SpyUtilityNW.Instance.Config.Debug);
            }
        }


        //public static MethodBase TargetMethod() => typeof(Scp079Camera).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly).First(method => method.Name.Contains("OnDestroy"));

        [PluginEvent(ServerEventType.TeamRespawn)]
        private void onWaveSpawn(SpawnableTeamType spawnTeamType)
        {
            Log.Debug(
                $"What is ServerEventType.TeamRespawn {spawnTeamType == SpawnableTeamType.NineTailedFox} and blah", SpyUtilityNW.Instance.Config.Debug);
        }

        [PluginEvent(ServerEventType.TeamRespawnSelected)]
        private void onWaveRespawn(SpawnableTeamType spawnTeamType)
        {
            Log.Debug("What is ServerEventType.TeamRespawnSelected and blah", SpyUtilityNW.Instance.Config.Debug);
        }
    }
}