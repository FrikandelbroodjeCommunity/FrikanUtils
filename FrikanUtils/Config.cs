using System.Collections.Generic;
using System.ComponentModel;
using MapGeneration.Holidays;

namespace FrikanUtils;

public class Config
{
    [Description("Whether to show debug messages")]
    public bool Debug { get; set; }

    [Description("Colors used for rainbows")]
    public string[] RainbowTextColors { get; set; } =
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
    public float HintRefreshTime { get; set; } = 0.5f;

    [Description("[Automatically generated] List of Server Specific Settings menus to assist in getting IDs")]
    public List<string> ServerSettingMenus { get; set; } = [];

    [Description("The text to display when the Server Specific Settings for a user is empty")]
    public string NoSettingsText { get; set; } = "It appears there is currently nothing to show you here.";

    [Description("Whether the improved thrown keycard detection is enabled")]
    public bool ImprovedCardDetection { get; set; } = true;

    [Description("Holiday override for debugging purposes. Makes the server think the event is currently running.")]
    public HolidayType OverrideHoliday { get; set; }
}