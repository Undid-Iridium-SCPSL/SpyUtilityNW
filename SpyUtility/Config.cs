using System.Collections.Generic;
using System.ComponentModel;
using SpyUtilityNW.Spies;

namespace SpyUtilityNW
{
    public class Config
    {
        /// <summary>
        /// ah
        /// </summary>
        [Description("Probability of spawning a CI spy per spy.")]
        public Dictionary<int, float> probabilityOfCISpy { get; set; } = new()
        {
            {1, 100f},
            {2, 50f},
            {3, 5f}
        };

        /// <summary>
        /// ah
        /// </summary>
        [Description("Probability of spawning a Mtf spy per spy.")]
        public Dictionary<int, float> probabilityOfMtfSpy { get; set; } = new()
        {
            {1, 70f},
            {2, 50f},
            {3, 5f}
        };
        
        /// <summary>
        /// Whether plugin is enabled or not.
        /// </summary>
        [Description("Whether plugin is enabled or not.")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether debug logs should be shown.
        /// </summary>
        [Description("Whether debug logs should be shown.")]
        public bool Debug { get; set; } = false;
        
        /// <summary>
        ///     Gets or sets a value determining how many attempts to spawn a spy can occur.
        /// </summary>
        [Description("Gets or sets a value determining how many attempts to spawn a spy can occur.")]
        public int HowManyRetries { get; set; } = 3;


        /// <summary>
        ///     Gets or sets a string that is shown when a spy is created.
        /// </summary>
        [Description("Gets or sets a string that is show when a spy is created. You must provide a {0} in your string to place the team name")]
        public string OnSpySpawnMessage { get; set; } =
            "<align=center><voffset=28em> <color=#F6511D> Congrats, you're a spy for {0} team </color></voffset></align>";
        
        /// <summary>
        /// How long to show <see cref="OnSpySpawnMessage"/> for.
        /// </summary>
        [Description("How long to show OnSpySpawnMessage for.")]
        public float OnSpySpawnMessageHintDuration { get; set; } = 3f;
        
        /// <summary>
        ///     Gets or sets a string that is shown when attacking a spy hiding on the enemy team
        /// </summary>
        [Description("Gets or sets a string that is shown when spy attacking real teammate. You must provide a {0} in your string to place the team name")]
        public string OnSpyAttackingTeammate { get; set; } =
            "<align=center><voffset=28em> <color=#F6511D> You're on {0} team, remember? </color></voffset></align>";
        
        /// <summary>
        /// How long to show <see cref="OnSpyAttackingTeammate"/> for.
        /// </summary>
        [Description("How long to show OnSpySpawnMessage for.")]
        public float OnSpyAttackingTeammateHintDuration { get; set; } = 3f;

        /// <summary>
        ///     Gets or sets a string that is shown when attacking a spy hiding on the enemy team
        /// </summary>
        [Description("Gets or sets a string that is shown when attacking a spy hiding on the enemy team. You must provide a {0} in your string to place the team name")]
        public string OnTeammateAttackingSpy { get; set; } = "<align=center><voffset=28em> <color=#F6511D> They're on {0} team, remember? </color></voffset></align>";

               
        /// <summary>
        /// How long to show <see cref="OnTeammateAttackingSpy"/> for.
        /// </summary>
        [Description("How long to show OnSpySpawnMessage for.")]
        public float OnTeammateAttackingSpyHintDuration { get; set; } = 3f;

        /// <summary>
        ///     Gets or sets a string that is shown when a spy is revealed. 
        /// </summary>
        [Description("Gets or sets a string that is shown when a spy is revealed.")]
        public string SpyHasBeenRevealed { get; set; } =
            "<align=center><voffset=28em> <color=#F6511D> You've been revealed!!! </color></voffset></align>";
        
        /// <summary>
        /// How long to show <see cref="SpyHasBeenRevealed"/> for.
        /// </summary>
        [Description("How long to show OnSpySpawnMessage for.")]
        public float SpyHasBeenRevealedHintDuration { get; set; } = 3f;


        /// <summary>
        ///     Gets or sets a string that is shown when a spy's on the same real team attack each other.
        /// </summary>
        [Description("Gets or sets a string that is shown when a spy's on the same real team attack each other.")]
        public string SameTeamSpyMessage { get; set; } =
            "<align=center><voffset=28em> <color=#F6511D> That's a fellow spy! You can't attack them </color></voffset></align>";
        
        /// <summary>
        /// How long to show <see cref="SpyHasBeenRevealed"/> for.
        /// </summary>
        [Description("How long to show OnSpySpawnMessage for.")]
        public float SameTeamSpyMessageHintDuration { get; set; } = 3f;

        /// <summary>
        /// CI Spy loadout <see cref="CiSpyBase"/>
        /// </summary>
        [Description("CI Spy loadout.")]
        public CiSpyBase CiSpyLoadout { get; set; } = new CiSpyBase();

        /// <summary>
        /// MTF Spy loadout <see cref="MtfSpyBase"/>
        /// </summary>
        [Description("MTF Spy loadout.")]
        public MtfSpyBase MtfSpyLoadout { get; set; } = new MtfSpyBase();

        /// <summary>
        /// How long to delay the respawn role change
        /// </summary>
        [Description("How long to delay the respawn role change.")]
        public float RespawnChangeRoleDelay { get; set; } = 0.05f;
        
        /// <summary>
        /// How long to delay before changing the spy loadout
        /// </summary>
        [Description("How long to delay the respawn role change.")]
        public float RespawnChangeRoleItemsDelay { get; set; } = 0.05f;

        /// <summary>
        /// How long to delay before changing the spy appearance
        /// </summary>
        [Description("How long to delay the appearance change.")]
        public float ChangeAppearanceDelay { get; set; } = 0.05f;

        /// <summary>
        /// Hide the unit names for respawning wave.
        /// </summary>
        [Description("Hide the unit names for respawning wave.")]
        public bool HideAllUnitNamesForRespawningTeam { get; set; } = false;

        /// <summary>
        /// Hide the unit names for respawning wave after X delay.
        /// </summary>
        [Description("Hide the unit names for respawning wave after X delay.")]
        public float HideAllUnitNamesForRespawningTeamDelay { get; set; } = 1f;

        /// <summary>
        /// Hide the unit names for spies.
        /// </summary>
        [Description("Hide the unit names for spies.")]
        public bool HideAllUnitNamesForSpies { get; set; } = false;
    }
}