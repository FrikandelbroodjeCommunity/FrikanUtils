using System.Collections.Generic;
using System.ComponentModel;

namespace FrikanUtils;

public class Config
{
    [Description("Whether to show debug messages")]
    public bool Debug { get; set; }

    [Description("[Automatically generated] List of Server Specific Settings fields to get IDs")]
    public List<string> ServerSettingIds { get; set; } = [];

    [Description("The text to display when the Server Specific Settings for a user is empty")]
    public string NoSettingsText = "It appears there is currently nothing to show you here.";
}