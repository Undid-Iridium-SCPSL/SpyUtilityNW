using System;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;

namespace SpawnManagementUtilityNW
{
    public class SpawnManagementUtility
    {
        [PluginConfig]
        public Config Config;
        
        /// <summary>
        /// Gets a static instance of the <see cref="SpawnManagementUtility"/> class.
        /// </summary>
        public static SpawnManagementUtility Instance { get; private set; }
        
        
        [PluginEntryPoint("SpawnManagementUtilityNW", "1.0.0", 
            "CLean up items by zone, item, and time", "Undid Iridium")]
        void LoadPlugin()
        {
            Instance = this;
       
            PluginAPI.Events.EventManager.RegisterEvents(this);
            Log.Debug("We have started our plugin CleanupUtilityNW!!", Instance.Config.Debug);
        }
        
    }
}