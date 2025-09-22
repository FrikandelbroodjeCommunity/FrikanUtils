# About the audio module

The audio module assists in playing music files through dummies or speakers.

> [!NOTE]
> Documentation for developers is still a work in progress and will be put in
> the [wiki](https://github.com/FrikandelbroodjeCommunity/FrikanUtils/wiki)
>
> Currently, most public methods within the code have documentation, which you can view on GitHub.

# Installation

> [!IMPORTANT]
> **Required dependencies:**
>  - [FrikanUtils](../FrikanUtils/README.md)
>  - [NVorbis](https://github.com/NVorbis/NVorbis/releases/tag/v0.10.5)

Place the [latest release](https://github.com/FrikandelbroodjeCommunity/FrikanUtils/releases/latest) for "
FrikanUtils_Audio.dll" in the LabAPI plugin folder.

# Features

This plugin is a library plugin which plugin developers can build on, most features below won't add anything on their
own.

## Speaker Audio Player

Allows a plugin to play audio from a file through a speaker. This does not require an NPC, instead playing the audio
from an invisible point.

## Hub Audio Player

Allows a plugin to player audio through a dummy. If no name for the dummy is provided, a default name will be used,
which can be set in the config.

## Global Settings

The mute setting allows each player to determine whether they want to mute the audio played through the speaker and hub
audio players. A plugin can override this if the audio has its own requirements.

The volume setting allows server staff to set the default value audio is played at. This is automatically synced with
the config. A plugin can override this if the audio needs its own volume.

# Config

| Config                  | Default        | Description                                                                                                 |
|-------------------------|----------------|-------------------------------------------------------------------------------------------------------------|
| `DefaultName`           | `Music Bot`    | The default name the bot uses, if the plugin does not specify a name, and no holiday is active.             |
| `DefaultHalloweenName`  | `Ghost`        | The default name the bot uses, if the plugin does not specify a name, and the Halloween holiday is active.  |
| `DefaultChristmasName`  | `Santa's elve` | The default name the bot uses, if the plugin does not specify a name, and the Christmas holiday is active.  |
| `DefaultAprilFoolsName` | `Herobrine`    | The default name the bot uses, if the plugin does not specify a name, and the AprilFools holiday is active. |
| `Volume`                | `5`            | The volume that is used by default, if the plugin does not specify a volume.                                |