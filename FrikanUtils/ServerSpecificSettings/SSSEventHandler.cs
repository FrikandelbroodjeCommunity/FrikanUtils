using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings;

internal static class SSSEventHandler
{
    internal static void RegisterEvents()
    {
        ServerSpecificSettingsSync.ServerOnStatusReceived += OnUpdateReceived;
        PlayerEvents.Joined += OnPlayerJoined;
        PlayerEvents.Left += OnPlayerLeft;
    }

    internal static void UnregisterEvents()
    {
        ServerSpecificSettingsSync.ServerOnStatusReceived -= OnUpdateReceived;
        PlayerEvents.Joined -= OnPlayerJoined;
        PlayerEvents.Left -= OnPlayerLeft;
    }

    private static void OnPlayerJoined(PlayerJoinedEventArgs ev)
    {
        Timing.CallDelayed(2f, () => { SSSHandler.CreatePlayer(ev.Player); });
    }

    private static void OnPlayerLeft(PlayerLeftEventArgs ev)
    {
        SSSHandler.DestroyPlayer(ev.Player);
    }

    private static void OnUpdateReceived(ReferenceHub hub, SSSUserStatusReport status)
    {
        var player = Player.Get(hub);

        if (SSSHandler.PlayerMenus.TryGetValue(player, out var menu))
        {
            menu.SetOpen(status.TabOpen);
        }
    }

    internal static void OnValueReceived(Player player, SettingsBase field, NetworkReaderPooled reader)
    {
        if (field is Button button)
        {
            field.Base.DeserializeValue(reader);
            button.OnClick?.Invoke(player);
        }
        else if (field is Dropdown dropdown)
        {
            var previous = dropdown.Value;
            field.Base.DeserializeValue(reader);
            dropdown.OnValueChanged(player, previous, ((SSDropdownSetting)dropdown.Base).SyncSelectionText);
        }
        else if (field is Keybind keybind)
        {
            field.Base.DeserializeValue(reader);

            var isPressed = ((SSKeybindSetting)keybind.Base).SyncIsPressed;
            // Only when we already received a value do we process the button presses
            // Otherwise we always get the button presses when receiving a value
            if (isPressed != keybind.PressedPreviousFrame)
            {
                if (isPressed)
                {
                    keybind.OnPressed?.Invoke(player);
                }
                else
                {
                    keybind.OnReleased?.Invoke(player);
                }
            }

            keybind.PressedPreviousFrame = isPressed;
        }
        else if (field is Slider slider)
        {
            // Do the int variants here as they are not done automatically
            if (!slider.ReceivedInitialValue)
            {
                slider.OnInitialValueInt?.Invoke(player, ((SSSliderSetting)slider.Base).SyncIntValue);
            }

            var previous = slider.Value;
            field.Base.DeserializeValue(reader);

            slider.OnChangedInt?.Invoke(player, ((SSSliderSetting)slider.Base).SyncIntValue);
            slider.OnValueChanged(player, previous, ((SSSliderSetting)slider.Base).SyncFloatValue);
        }
        else if (field is TextInput text)
        {
            var previous = text.Value;
            field.Base.DeserializeValue(reader);

            text.OnValueChanged(player, previous, ((SSPlaintextSetting)text.Base).SyncInputText);
        }
        else if (field is TwoButtonSetting twoButton)
        {
            var previous = twoButton.Value;
            field.Base.DeserializeValue(reader);

            twoButton.OnValueChanged(player, previous, ((SSTwoButtonsSetting)twoButton.Base).SyncIsB);
        }
    }
}