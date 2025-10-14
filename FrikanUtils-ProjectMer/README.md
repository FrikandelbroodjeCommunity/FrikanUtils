# About the project MER module

The Project MER module assists in loading schematics and spawning them.

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

## Adding ProjectMER files

The Project MER module uses the [FrikanUtils file system](../FrikanUtils/README.md). By default, you can add your
ProjectMER schematics in the `/LabAPI/configs/{port/global}/FrikanUtils/Maps` folder.

If you use any custom file providers, you can also add the file to the location accessed by your custom file provider.

# Features

This plugin is a library plugin which plugin developers can build on, most features below won't add anything on their
own.

## Spawn System

The utilities contain some helper methods to help developers with spawning MER maps using
the [File System](../FrikanUtils/README.md#file-system).

The following helpers which can be used on spawned maps are included:

- Searching: The search methods allows a plugin to iterate over all objects with a given name.
- Doors: The door system allows doors to be spawned on all objects which have `SpawnDoor` in their name.
- Interactable: Allows the addition of an interactable component onto an already existing GameObject.

## Holiday System

Allows for certain objects to only be spawned when a holiday is active. In order to use this you must follow the naming
convention `{holidays};{name}` where `{holidays}` are the, comma separated, holidays for which this block, and all child
blocks, should be spawned and `{name}` can be anything you want.

An example would be `Christmas,Halloween;window`, which would be an object named window that would only spawn when the
christmas or halloween
holiday is active.