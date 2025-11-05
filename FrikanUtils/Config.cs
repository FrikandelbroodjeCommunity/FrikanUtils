using System.Collections.Generic;
using System.ComponentModel;
using MapGeneration.Holidays;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace FrikanUtils;

public class Config
{
    [Description("Whether to show debug messages")]
    public bool Debug { get; set; }

    [Description("Whether the custom dummy actions system should be enabled")]
    public bool UseCustomDummyActions { get; set; } = true;

    [Description("Whether the hint system should be enabled")]
    public bool UseHintSystem { get; set; } = true;

    [Description("Whether the keycard improvements should be enabled")]
    public bool UseKeycardImprovements { get; set; } = true;

    [Description("Whether the server specific settings system should be enabled")]
    public bool UseServerSpecificSettings { get; set; } = true;

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

    [Description("The amount of refreshes the same rainbow color is shown")]
    public int RainbowColorTicks { get; set; } = 2;

    [Description("Time between updates of the continuous hints")]
    public float HintRefreshTime { get; set; } = 0.5f;

    [Description("Text shown while in the lobby, leave empty to not show any text by default")]
    public string LobbyText { get; set; } = "<color=rainbow><b>[Server rules]</b></color>\n" +
                                            "Please read our server rules! You can find them in our server info.\n" +
                                            "<i>Esc -> Server Info</i>\n\n" +
                                            "<color=rainbow><b>[Server settings]</b></color>\n" +
                                            "This server uses the Server Specific Settings system.\n" +
                                            "Go and check it out in your settings menu!\n" +
                                            "<i>Esc -> Settings -> Server-specific</i></size>";


    [Description("The text to display when the Server Specific Settings for a user is empty")]
    public string NoSettingsText { get; set; } = "It appears there is currently nothing to show you here.";

    [Description("Holiday override for debugging purposes. Makes the server think the event is currently running.")]
    public HolidayType OverrideHoliday { get; set; }

    [Description("URL to use for the remote file provider. Before using this, read the owner manual" +
                 "(https://github.com/FrikandelbroodjeCommunity/FrikanUtils/wiki/Using-the-File-System)")]
    public string RemoteFileProviderUrl { get; set; } = "";

    [Description("Whether the remote file provider should attempt to find holiday files. " +
                 "This may cause additional requests if no holiday file exists and it needs to search for the original file.")]
    public bool RemoteFileProviderUsesHolidays { get; set; } = false;

    [Description("[Automatically generated] List of Server Specific Settings menus to assist in getting IDs")]
    public List<string> ServerSettingMenus { get; set; } = [];

    [Description("[Automatically generated] List of Global Client settings, " +
                 "ensuring they are always given the same ID")]
    public List<string> GlobalClientSettings { get; set; } = [];
}