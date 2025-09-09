using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings;

public static class SSSEventHandler
{
    public static void RegisterEvents()
    {
        PlayerEvents.Joined += OnPlayerJoined;
        PlayerEvents.Left += OnPlayerLeft;
    }

    public static void UnregisterEvents()
    {
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

    internal static void OnUpdateReceived(ReferenceHub hub, SSSUserStatusReport status)
    {
        var player = Player.Get(hub);

        if (SSSHandler.PlayerMenus.TryGetValue(player, out var menu))
        {
            menu.SetOpen(status.TabOpen);
        }
    }

    internal static void OnValueReceived(Player player, SettingsBase field)
    {
        if (field is Button button)
        {
            button.OnClick?.Invoke(player);
        }
        else if (field is Dropdown dropdown)
        {
            if (!dropdown.ReceivedInitialValue)
            {
                dropdown.ReceivedInitialValue = true;
                dropdown.OnInitialValue?.Invoke(player, ((SSDropdownSetting)dropdown.Base).SyncSelectionText);
            }

            dropdown.OnChanged?.Invoke(player, ((SSDropdownSetting)dropdown.Base).SyncSelectionText);
        }
        else if (field is Keybind keybind)
        {
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
            if (!slider.ReceivedInitialValue)
            {
                slider.ReceivedInitialValue = true;
                slider.OnInitialValue?.Invoke(player, ((SSSliderSetting)slider.Base).SyncFloatValue);
                slider.OnInitialValueInt?.Invoke(player, ((SSSliderSetting)slider.Base).SyncIntValue);
            }

            slider.OnChanged?.Invoke(player, ((SSSliderSetting)slider.Base).SyncFloatValue);
            slider.OnChangedInt?.Invoke(player, ((SSSliderSetting)slider.Base).SyncIntValue);
        }
        else if (field is TextInput text)
        {
            if (!text.ReceivedInitialValue)
            {
                text.ReceivedInitialValue = true;
                text.OnInitialValue?.Invoke(player, ((SSPlaintextSetting)text.Base).SyncInputText);
            }

            text.OnChanged?.Invoke(player, ((SSPlaintextSetting)text.Base).SyncInputText);
        }
        else if (field is TwoButtonSetting twoButton)
        {
            twoButton.OnChanged?.Invoke(player, ((SSTwoButtonsSetting)twoButton.Base).SyncIsB);
        }
    }
}