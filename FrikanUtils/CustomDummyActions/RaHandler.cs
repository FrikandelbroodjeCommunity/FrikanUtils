using System.Collections.Generic;
using LabApi.Features.Console;

namespace FrikanUtils.CustomDummyActions;

/// <summary>
/// Handles the custom dummy system.
/// You should register the modules you want to be visible here.
/// </summary>
public static class RaHandler
{
    internal static readonly List<DummyModuleBase> Modules = [];

    /// <summary>
    /// Registers a dummy module.
    /// If a duplicate is found (another module with the same name),
    /// the duplicate will be removed and this module added.
    /// </summary>
    /// <param name="module">Module to add</param>
    public static void RegisterModule(DummyModuleBase module)
    {
        if (UnregisterModule(module))
        {
            Logger.Warn("Found multiple dummy modules with the same name, overwriting the previous!");
        }

        Modules.Add(module);
    }

    /// <summary>
    /// Remove a module from the system. The module can no longer show up or used.
    /// Will return false if it failed to remove the module, or the module is not in the list.
    /// </summary>
    /// <param name="module">Module to remove</param>
    /// <returns>Whether the module was removed</returns>
    public static bool UnregisterModule(DummyModuleBase module)
    {
        return Modules.Remove(module);
    }
}