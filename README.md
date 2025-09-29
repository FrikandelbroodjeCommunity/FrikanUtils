[![GitHub release](https://flat.badgen.net/github/release/FrikandelbroodjeCommunity/FrikanUtils/)](https://github.com/FrikandelbroodjeCommunity/FrikanUtils/releases/latest)
[![LabAPI Version](https://flat.badgen.net/static/LabAPI%20Version/v1.1.3)](https://github.com/northwood-studios/LabAPI)
[![License](https://flat.badgen.net/github/license/FrikandelbroodjeCommunity/FrikanUtils/)](https://github.com/FrikandelbroodjeCommunity/FrikanUtils/blob/master/LICENSE)

# About FrikanUtils

The goal of FrikanUtils is to fill gaps in the base game and LabAPI, attempting to reduce the amount of code that needs
to be copied between different projects.

The FrikanUtils project consists of multiple plugins called "modules". The main module provides the foundation of the
other modules, so is always required. The split into multiple modules was done to allow server owners to only install
the required modules, reducing the amount of dependencies that need to be installed.

The development on the initial version of this started when LabAPI was first released, so some (parts of) modules may
conflict with other plugins.

# Modules

| Module                                         | Features                                                                                                                                                                                                                                                                                                                                                                                                                            |
|------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [FrikanUtils](FrikanUtils/README.md)           | **This is the base module, and is required for all other modules.**<br/> - Custom remote admin dummy actions<br/> - FileSystem to assist in getting files<br/> - HintSystem to help with hints that need to be displayed permanently<br/> - Custom keycard system to help with modifying custom keycards<br/> - Common implementations for NPCs<br/> - Advanced Server Specific Settings system<br/> - Various additional utilities |
| [Audio](FrikanUtils-Audio/README.md)           | Utilities to play audio files in the game<br/> - Play audio through dummies<br/> - Player audio through speakers                                                                                                                                                                                                                                                                                                                    |
| [ProjectMER](FrikanUtils-ProjectMer/README.md) | Contains some functions to help with using ProjectMER. The utilities provided here can be used on normal ProjectMER files, they require no additional content in Unity.<br/> - Seasonal maps<br/> - Trigger helper<br/> - Block finder using name<br/> - Making blocks interactable                                                                                                                                                 |

# Future plans

Below is a list of features that may be added in the future, with a small description. These future plans are **not** a
promise that something will be implemented.

| Feature              | Description                                                                                                                                                                 |
|----------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| More (advanced) NPCs | - Extension of Follow NPCs which can actively wander around the map until they find another player, which they will then target.<br/> - No further ideas (yet)              |
| Localization         | Allow plugins to register localizations both for their own plugins, but also other plugins, allowing players to choose a language from their Server Specific Settings menu. |
| Centralized config   | Functionality that allows for/makes it easier for a centralized configuration, allowing multiple servers to synchronize their configuration with a central server.          |
| Bridges              | Add bridge modules that allow FrikanUtils to work together with other SSS or Dummy Action frameworks.                                                                       |
