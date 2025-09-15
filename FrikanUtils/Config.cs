using System.Collections.Generic;
using System.ComponentModel;

namespace FrikanUtils;

public class Config
{
    [Description("Whether to show debug messages")]
    public bool Debug { get; set; }

    [Description("Colors used for rainbows")]
    public string[] RainbowTextColors =
    [
        "red",
        "#FF9500",
        "yellow",
        "green",
        "#00FFF6",
        "blue",
        "#FF00FF"
    ];

    [Description("Time between updates of the continuous hints")]
    public float HintRefreshTime = 0.5f;

    [Description("[Automatically generated] List of Server Specific Settings fields to get IDs")]
    public List<string> ServerSettingIds { get; set; } = [];

    [Description("The text to display when the Server Specific Settings for a user is empty")]
    public string NoSettingsText = "It appears there is currently nothing to show you here.";

    [Description("Whether or not the improved thrown keycard detection is enabled")]
    public bool ImprovedCardDetection { get; set; } = true;
}