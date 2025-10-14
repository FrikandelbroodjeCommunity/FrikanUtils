using System.ComponentModel;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace FrikanUtils;

public class AudioConfig
{
    [Description("Default audio bot name")]
    public string DefaultName { get; set; } = "Music Bot";

    [Description("Default audio bot name during halloween")]
    public string DefaultHalloweenName { get; set; } = "Ghost";

    [Description("Default audio bot name during christmas")]
    public string DefaultChristmasName { get; set; } = "Santa's elve";

    [Description("Default audio bot anme during april fools")]
    public string DefaultAprilFoolsName { get; set; } = "Herobrine";

    [Description("Default volume of the music bot")]
    public float Volume { get; set; } = 5f;
}