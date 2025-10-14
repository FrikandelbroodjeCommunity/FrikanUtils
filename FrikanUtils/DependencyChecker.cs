using System;
using LabApi.Features.Console;
using NVorbis;
using ProjectMER.Features.Serializable;

namespace FrikanUtils;

internal static class DependencyChecker
{
    public static bool AudioModule;
    public static bool ProjectMerModule;

    public static void CheckDependencies()
    {
        AudioModule = CheckDependency("Audio Module", CheckAudioModule);
        ProjectMerModule = CheckDependency("Project MER Module", CheckProjectMerModule);
    }

    private static bool CheckDependency(string name, Action func)
    {
        try
        {
            func.Invoke();
        }
        catch
        {
            Logger.Warn($"Dependencies for {name} are not all present. The {name} can not be used.");
            return false;
        }

        Logger.Info($"Dependencies for {name} are found. The {name} can be used.");
        return true;
    }

    private static void CheckAudioModule() => _ = typeof(VorbisReader);
    private static void CheckProjectMerModule() => _ = new MapSchematic();
}