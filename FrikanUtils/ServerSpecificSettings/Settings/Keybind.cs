using System;
using LabApi.Features.Wrappers;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public class Keybind : SettingsBase
{
    public override ServerSpecificSettingBase Base { get; }

    public bool IsPressed => ((SSKeybindSetting)Base).SyncIsPressed;

    internal Action<Player> OnPressed;
    internal Action<Player> OnReleased;
    internal bool PressedPreviousFrame;

    public Keybind(
        ushort? id,
        string label,
        KeyCode suggestedKey = KeyCode.None,
        bool preventInteractionOnGui = true,
        bool allowSpectatorTrigger = true,
        string hint = null) : base(id)
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

    public Keybind RegisterPressedAction(Action<Player> pressedAction)
    {
        OnPressed = pressedAction;
        return this;
    }

    public Keybind RegisterReleasedAction(Action<Player> releasedAction)
    {
        OnReleased = releasedAction;
        return this;
    }

    public override SettingsBase Clone()
    {
        var keybind = (SSKeybindSetting)Base;
        return new Keybind(SettingId, Label, keybind.SuggestedKey, keybind.PreventInteractionOnGUI,
                keybind.AllowSpectatorTrigger, HintDescription)
            .RegisterPressedAction(OnPressed)
            .RegisterReleasedAction(OnReleased);
    }
}