using FrikanUtils.ServerSpecificSettings.Helpers;
using LabApi.Features.Console;

namespace FrikanUtils.ServerSpecificSettings;

public class PlayerIdHandler
{
    private int _internalIdCounter = SSSHandler.LowestReservedId;

    public int GetId(string menuId, ushort? fieldId)
    {
        if (!fieldId.HasValue)
        {
            // Remove one from the internal counter and return the new value.
            return --_internalIdCounter;
        }

        var menuIndex = UtilitiesPlugin.PluginConfig.ServerSettingMenus.FindIndex(x => x == menuId);
        if (menuIndex == -1) // If we did not find an entry, add one
        {
            menuIndex = UtilitiesPlugin.PluginConfig.ServerSettingMenus.Count;
            UtilitiesPlugin.PluginConfig.ServerSettingMenus.Add(menuId);

            Logger.Debug($"Could not find the menu ID: {menuId}, adding to the config",
                UtilitiesPlugin.PluginConfig.Debug);

            // Save the config
            UtilitiesPlugin.Save();
        }

        return menuIndex << 16 | fieldId.Value;
    }

    public void Reset()
    {
        _internalIdCounter = SSSHandler.LowestReservedId;
    }
}