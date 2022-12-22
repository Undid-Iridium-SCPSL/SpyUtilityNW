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
 
![NVIDIA_Share_YmibdG6PY2](https://user-images.githubusercontent.com/24619207/163738277-e2a80193-5ae2-497e-99fd-181468e7742f.png)
![NVIDIA_Share_5ZWKPjTGmo](https://user-images.githubusercontent.com/24619207/163738279-76834f94-42ee-4bc6-845a-6eca3a60d577.png)
![NVIDIA_Share_2TTTAws7Dt](https://user-images.githubusercontent.com/24619207/163738278-5dc8afe0-9dbe-4e02-92ca-c9056e57c369.png)

(The gap in time is ridtp to surface and spawning the items in)

![image](https://user-images.githubusercontent.com/24619207/163898085-097de715-450f-47b9-adc1-ed5d019f789a.png)
