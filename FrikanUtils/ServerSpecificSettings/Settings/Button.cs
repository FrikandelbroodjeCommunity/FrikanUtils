using System;
using FrikanUtils.ServerSpecificSettings.Helpers;
using LabApi.Features.Wrappers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

/// <summary>
/// Button for Server Specific Settings.
/// </summary>
public class Button : SettingsBase
{
    /// <inheritdoc />
    public override ServerSpecificSettingBase Base => Setting;

    /// <summary>
    /// The button setting used by the base game.
    /// </summary>
    public readonly SSButton Setting;

    /// <summary>
    /// The time the button needs to be held in order to register a click.
    /// </summary>
    public float HoldTime
    {
        get => Setting.HoldTimeSeconds;
        set => Update(null, value);
    }

    /// <summary>
    /// The text shown on the button.
    /// </summary>
    public string ButtonText
    {
        get => Setting.ButtonText;
        set => Update(value);
    }

    internal Action<Player> OnClick;

    /// <summary>
    /// Create a new button with the given settings.
    /// Buttons are always <see cref="ServerOnlyType.ServerOnly"/>.
    /// </summary>
    /// <param name="id">Optional ID that can be used to refer to this setting</param>
    /// <param name="label">Label shown for this setting</param>
    /// <param name="buttonText">Text shown on the clickable button</param>
    /// <param name="holdTimeSeconds">The time the button needs to be held before registering a click</param>
    /// <param name="hint">Additional explanation for the button</param>
    public Button(
        ushort? id,
        string label,
        string buttonText,
        float? holdTimeSeconds = null,
        string hint = null) : base(id, ServerOnlyType.ServerOnly)
    {
        Setting = new SSButton(
            null,
            label,
            buttonText,
            holdTimeSeconds,
            hint
        );
    }

    /// <summary>
    /// Update the text shown on the button or the hold time.
    /// When one is not set, the currently stored value will be used.
    /// </summary>
    /// <param name="newText">The new text to display on the button</param>
    /// <param name="newHoldTime">The new time the button needs to be held</param>
    /// <param name="applyOverride">Whether to apply the change immediately</param>
    public void Update(string newText = null, float? newHoldTime = null, bool applyOverride = true)
    {
        Setting.SendButtonUpdate(newText ?? Setting.ButtonText, newHoldTime ?? Setting.HoldTimeSeconds, applyOverride,
            UpdateFilter);
    }

    /// <summary>
    /// Sets the action that should be executed when a player clicks on the button.
    /// </summary>
    /// <param name="clickedAction">Action to execute on click</param>
    /// <returns>This button instance, allowing the chaining of operations</returns>
    public Button RegisterClickAction(Action<Player> clickedAction)
    {
        OnClick = clickedAction;
        return this;
    }
}