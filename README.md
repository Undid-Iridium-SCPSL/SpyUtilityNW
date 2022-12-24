# SpyUtilityNW
Ability to have MTF and Chaos spies. 



![SpyUtilityNW ISSUES](https://img.shields.io/github/issues/Undid-Iridium/SpyUtilityNW)
![SpyUtilityNW FORKS](https://img.shields.io/github/forks/Undid-Iridium/SpyUtilityNW)
![SpyUtilityNW LICENSE](https://img.shields.io/github/license/Undid-Iridium/SpyUtilityNW)


![SpyUtilityNW LATEST](https://img.shields.io/github/v/release/Undid-Iridium/SpyUtilityNW?include_prereleases&style=flat-square)
![SpyUtilityNW LINES](https://img.shields.io/tokei/lines/github/Undid-Iridium/SpyUtilityNW)
![SpyUtilityNW DOWNLOADS](https://img.shields.io/github/downloads/Undid-Iridium/SpyUtilityNW/total?style=flat-square)


# SpyUtilityNW

Ability to add spies (MTF/CI) to the game. 

TODO's (maybe):
Scientist/D-Class/Tutorial spies?
Allow configuration of message to player.

# Installation

**[NWAPI](https://github.com/northwood-studios/NwPluginAPI) must be installed for this to work.**

**[Harmony 2.2.2](https://github.com/pardeike/Harmony/releases/tag/v2.2.2.0)**

## REQUIREMENTS
* NWAPI: V12.0.0-RC.2
* SCP:SL Server: V12
* Harmony 2.2.2



Example configuration
```
# Probability of spawning a CI spy per spy.
probability_of_c_i_spy:
  1: 100
  2: 50
  3: 5
# Probability of spawning a Mtf spy per spy.
probability_of_mtf_spy:
  1: 100
  2: 50
  3: 5
# Whether plugin is enabled or not.
is_enabled: true
# Whether debug logs should be shown.
debug: true
# Gets or sets a value determining how many attempts to spawn a spy can occur.
how_many_retries: 3
# Gets or sets a string that is show when a spy is created. You must provide a {0} in your string to place the team name
on_spy_spawn_message: <align=center><voffset=28em> <color=#F6511D> Congrats, you're a spy for {0} team </color></voffset></align>
# How long to show OnSpySpawnMessage for.
on_spy_spawn_message_hint_duration: 3
# Gets or sets a string that is shown when spy attacking real teammate. You must provide a {0} in your string to place the team name
on_spy_attacking_teammate: <align=center><voffset=28em> <color=#F6511D> You're on {0} team, remember? </color></voffset></align>
# How long to show OnSpySpawnMessage for.
on_spy_attacking_teammate_hint_duration: 3
# Gets or sets a string that is shown when attacking a spy hiding on the enemy team. You must provide a {0} in your string to place the team name
on_teammate_attacking_spy: <align=center><voffset=28em> <color=#F6511D> They're on {0} team, remember? </color></voffset></align>
# How long to show OnSpySpawnMessage for.
on_teammate_attacking_spy_hint_duration: 3
# Gets or sets a string that is shown when a spy is revealed.
spy_has_been_revealed: <align=center><voffset=28em> <color=#F6511D> You've been revealed!!! </color></voffset></align>
# How long to show OnSpySpawnMessage for.
spy_has_been_revealed_hint_duration: 3
# Gets or sets a string that is shown when a spy's on the same real team attack each other.
same_team_spy_message: <align=center><voffset=28em> <color=#F6511D> That's a fellow spy! You can't attack them </color></voffset></align>
# How long to show OnSpySpawnMessage for.
same_team_spy_message_hint_duration: 3
# CI Spy loadout.
ci_spy_loadout:
# Default items for spy
  spy_starting_items:
  - ArmorHeavy
  - Radio
  - GrenadeHE
  - Medkit
  - Adrenaline
  - GunE11SR
  - KeycardNTFLieutenant
  - KeycardChaosInsurgency
  # Default ammo for spy
  spy_starting_ammo:
    Ammo9x19: 40
    Ammo556x45: 120
  # Override spy spawn position (Vector3). I DO NOT RECOMMEND.
  spawn_position:
  - 0
  - 0
  - 0
  # The fake role the spy will be, aka, what they will look like until exposed
  spy_fake_role: NtfSergeant
  # The real role the spy will be, aka, what they will look like after exposed
  spy_real_role: ChaosRifleman
# MTF Spy loadout.
mtf_spy_loadout:
# Default items for spy
  spy_starting_items:
  - ArmorCombat
  - Painkillers
  - Medkit
  - GunAK
  - KeycardChaosInsurgency
  - GrenadeHE
  # Default ammo for spy
  spy_starting_ammo:
    Ammo762x39: 120
  # Override spy spawn position (Vector3). I DO NOT RECOMMEND.
  spawn_position:
  - 0
  - 0
  - 0
  # The fake role the spy will be, aka, what they will look like until exposed
  spy_fake_role: ChaosRifleman
  # The real role the spy will be, aka, what they will look like after exposed
  spy_real_role: NtfSergeant


 ```
 ![NVIDIA_Share_cWpADdebzt](https://user-images.githubusercontent.com/24619207/209059478-0fd54345-3cd3-4edd-8b31-6715989d0027.png)

