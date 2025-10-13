using System;
using FrikanUtils.ServerSpecificSettings.Helpers;
using LabApi.Features.Wrappers;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

/// <summary>
/// Keybind for Server Specific Settings.
/// </summary>
public class Keybind : SettingsBase
{
    /// <inheritdoc />
    public override ServerSpecificSettingBase Base { get; }

    /// <summary>
    /// Whether the keybind is currently pressed by the player.
    /// </summary>
    public bool IsPressed => ((SSKeybindSetting)Base).SyncIsPressed;

    internal Action<Player> OnPressed;
    internal Action<Player> OnReleased;
    internal bool PressedPreviousFrame;

    /// <summary>
    /// Create a new keybind with the given settings.
    /// Keybinds are always <see cref="ServerOnlyType.Client"/>.
    /// </summary>
    /// <param name="id">Optional ID that can be used to refer to this setting</param>
    /// <param name="label">Label shown for this setting</param>
    /// <param name="suggestedKey">The key suggested to use for this keybind</param>
    /// <param name="preventInteractionOnGui">Whether the keybind should trigger when there was GUI</param>
    /// <param name="allowSpectatorTrigger">Whether the keybind can get triggered in spectator</param>
    /// <param name="hint">Additional explanation for the button</param>
    public Keybind(
        ushort? id,
        string label,
        KeyCode suggestedKey = KeyCode.None,
        bool preventInteractionOnGui = true,
        bool allowSpectatorTrigger = true,
        string hint = null) : base(id, ServerOnlyType.Client)
    {
        Base = new SSKeybindSetting(
            null,
            label,
            suggestedKey,
            preventInteractionOnGui,
            allowSpectatorTrigger,
            hint
        );
    }

    /// <summary>
    /// Register the action that needs to be executed when the keybind is pressed down.
    /// </summary>
    /// <param name="pressedAction">Action to be executed when pressing</param>
    /// <returns>This keybind instance, allowing the chaining of operations</returns>
    public Keybind RegisterPressedAction(Action<Player> pressedAction)
    {
        OnPressed = pressedAction;
        return this;
    }

    /// <summary>
    /// Register the action that needs to be executed when the keybind is released.
    /// </summary>
    /// <param name="releasedAction">Action to be executed when releasing</param>
    /// <returns>This keybind instance, allowing the chaining of operations</returns>
    public Keybind RegisterReleasedAction(Action<Player> releasedAction)
    {
        OnReleased = releasedAction;
        return this;
    }
}