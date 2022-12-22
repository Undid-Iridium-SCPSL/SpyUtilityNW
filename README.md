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
  1: 70
  2: 50
  3: 5
# Whether plugin is enabled or not.
is_enabled: true
# Whether debug logs should be shown.
debug: false
# Gets or sets a value determining how many attempts to spawn a spy can occur.
how_many_retries: 3

 ```
 ![NVIDIA_Share_cWpADdebzt](https://user-images.githubusercontent.com/24619207/209059478-0fd54345-3cd3-4edd-8b31-6715989d0027.png)

