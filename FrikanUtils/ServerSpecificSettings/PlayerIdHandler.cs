namespace FrikanUtils.ServerSpecificSettings;

public class PlayerIdHandler
{
    private int _internalIdCounter = SSSHandler.LowestReservedId;

    public bool TryGetId(string menuId, string fieldId, bool isServerOnly, out int id)
    {
        if (fieldId == null || isServerOnly)
        {
            // Remove one from the internal counter and return the new value.
            id = --_internalIdCounter;
            return true;
        }

        var full = $"{menuId}|{fieldId}";
        var index = UtilitiesPlugin.ServerSettingIds.FindIndex(x => x == full);
        if (index == -1) // If we did not find an entry, add one
        {
            index = UtilitiesPlugin.ServerSettingIds.Count;
            UtilitiesPlugin.ServerSettingIds.Add(full);

            // Save the config
            UtilitiesPlugin.Save();
        }

        id = index;
        return true;
    }

    public void Reset()
    {
        _internalIdCounter = SSSHandler.LowestReservedId;
    }
}