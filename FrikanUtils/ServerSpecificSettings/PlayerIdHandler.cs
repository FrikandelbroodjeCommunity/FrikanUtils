namespace FrikanUtils.ServerSpecificSettings;

public class PlayerIdHandler
{
    private int _internalIdCounter = SSSHandler.LowestReservedId;

    public int GetId(string menuId, byte? fieldId)
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

            // Save the config
            UtilitiesPlugin.Save();
        }

        return menuIndex << 8 | fieldId.Value;
    }

    public void Reset()
    {
        _internalIdCounter = SSSHandler.LowestReservedId;
    }
}