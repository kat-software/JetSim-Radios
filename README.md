
<h1 align="center">JetSim - Radios</h1>

<div align=center>
  <a href="https://github.com/kat-software/JetSim-Radios/actions"><img alt="GitHub Actions" src="https://img.shields.io/github/actions/workflow/status/kat-software/JetSim-Radios/release.yml?style=for-the-badge"></a>
  <a href="https://github.com/kat-software/JetSim-Radios?tab=MIT-1-ov-file"><img alt="GitHub license" src="https://img.shields.io/github/license/kat-software/JetSim-Radios?color=blue&style=for-the-badge"></a>
  <a href="https://github.com/kat-software/JetSim-Radios/releases/latest/"><img alt="GitHub latest release" src="https://img.shields.io/github/v/release/kat-software/JetSim-Radios?logo=unity&style=for-the-badge"></a>
  <a href="https://github.com/kat-software/JetSim-Radios/releases/"><img alt="GitHub all releases" src="https://img.shields.io/github/downloads/kat-software/JetSim-Radios/total?color=blue&style=for-the-badge"></a>
</div>

![JetSim](https://raw.githubusercontent.com/KitKat4191/JetSim-VCC-Listing/main/Website/banner.png)

## Dependencies

* Cyan Player Object Pool [VCC](https://cyanlaser.github.io/CyanPlayerObjectPool/), [GitHub](https://github.com/CyanLaser/CyanPlayerObjectPool)
* VRRefAssist [VCC](https://livedimensions.github.io/VRRefAssist/), [GitHub](https://github.com/LiveDimensions/VRRefAssist).

___

## Installation instructions

1. Install the dependencies
2. Add this package `JetSim - Radios` to your project by following one of the options below

### Option 1: VCC

* Add the following listings to the creator companion
* [VRRefAssist VCC listing](https://livedimensions.github.io/VRRefAssist/)
* [Cyan Player Object Pool VCC listing](https://cyanlaser.github.io/CyanPlayerObjectPool/)
* [JetSim VCC listing](https://kitkat4191.github.io/JetSim-VCC-Listing/)
* Manage Project > JetSim - Radios > Add package (+)
* The VCC *should* install the dependencies for you as long as the listings were added

### Option 2: UPM

* On the top bar in Unity click `Window > Package Manager`
* Click the `[+]` in the top left of the `Package Manager` window
* Select `Add package from git URL...` in the dropdown menu
* Paste this link: `https://github.com/kat-software/JetSim-Radios.git`
* Click `Add` on the right side of the link input field

### Option 3: `.unitypackage`

* Download the `.unitypackage` from the [latest release](https://github.com/kat-software/JetSim-Radios/releases/latest)
* Drag the `.unitypackage` from your downloads folder to the `Project` tab in your open Unity project

---

## Prefabs

You can find them at `Packages > JetSim - Radios > Prefabs`

### Radio System

There has to be one instance of this prefab in your scene. If it's not present the system won't work.

### Radio Activator

When this GameObject is enabled or disabled it will enable or disable the radio.

### Radio Zone

Enables the radio while the player is inside the trigger collider. You can use any convex trigger collider.

---

## Scripting API

### RadioManager Singleton

`class KatSoftware.JetSim.Radios.Runtime.RadioManager`

#### Referencing the singleton

```cs
using UnityEngine;

public class YourClass : UdonSharpBehaviour
{
    [SerializeField, HideInInspector] private RadioManager radioManager; // This reference is set automatically by VRRefAssist.
}
```

### GET

| Type    | Name                 | Summary                                                                                                                     |
|---------|----------------------|-----------------------------------------------------------------------------------------------------------------------------|
| `bool`  | `RadioEnabled`       | If the Radio system is both powered and enabled. This is the requirement for the player being able to transmit and receive. |
| `bool`  | `RadioPowered`       | For example if the player is in a RadioZone, a RadioActivator is enabled, or whatever you as the creator decide.            |
| `bool`  | `RadioSystemEnabled` | The player's preference for if they want to use the radio system.                                                           |
| `float` | `Volume`             | https://feedback.vrchat.com/udon/p/setvoicegain-does-not-set-voice-gain                                                     |
| `int`   | `Channel`            | The current channel the player is transmitting and listening on. Range is 0 to `MAX_CHANNEL`.                               |
| `int`   | `MAX_CHANNEL`        | The last selectable channel.                                                                                                |

### SET

| Type   | Name                                | Summary                                                                                                                                                                                             |
|--------|-------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `void` | `ToggleRadioPowered()`              |                                                                                                                                                                                                     |
| `void` | `SetRadioPowered(bool)`             | Used in RadioZone and RadioActivator.                                                                                                                                                               |
| `void` | `ToggleRadioSystemEnabled()`        |                                                                                                                                                                                                     |
| `void` | `SetRadioSystemEnabled(bool)`       | Used in the radio settings menu.                                                                                                                                                                    |
| `void` | `SetVolume(float)`                  | ~~Sets the volume of all remote voices on the radio. Range is 0 (off) to 1 (max).~~ https://feedback.vrchat.com/udon/p/setvoicegain-does-not-set-voice-gain                                         |
| `void` | `IncreaseChannel(bool wrap = true)` | Shorthand for `SetChannel(Channel + 1, wrap);`                                                                                                                                                      |
| `void` | `DecreaseChannel(bool wrap = true)` | Shorthand for `SetChannel(Channel - 1, wrap);`                                                                                                                                                      |
| `void` | `SetChannel(int, bool wrap = true)` | Sets the current channel the player is transmitting and listening on. Range is 0 to MAX_CHANNEL. The optional parameter `wrap` determines if the channel value should loop around instead of clamp. |

[![GitHub forks](https://img.shields.io/github/forks/kat-software/JetSim-Radios.svg?style=social&label=Fork)](https://github.com/kat-software/JetSim-Radios/fork) [![GitHub stars](https://img.shields.io/github/stars/kat-software/JetSim-Radios.svg?style=social&label=Stars)](https://github.com/kat-software/JetSim-Radios/stargazers)
