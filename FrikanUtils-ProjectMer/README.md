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
>  - [ProjectMER](https://github.com/Michal78900/ProjectMER/releases/latest)

Place the [latest release](https://github.com/FrikandelbroodjeCommunity/FrikanUtils/releases/latest) for "
FrikanUtils_ProjectMer.dll" in the LabAPI plugin folder.

# Features

This plugin is a library plugin which plugin developers can build on, most features below won't add anything on their
own.

## Spawn System

The utilities contain some helper methods to help developers with spawning MER maps using the [File System](../FrikanUtils/README.md#file-system).

The following helpers which can be used on spawned maps are included:
 - Searching: The search methods allows a plugin to iterate over all objects with a given name.
 - Doors: The door system allows doors to be spawned on all objects which have `SpawnDoor` in their name.
 - Interactable: Allows the addition of an interactable component onto an already existing GameObject.

## Doors System

## Holiday System

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
