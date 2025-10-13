using System;
using System.Collections.Concurrent;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace FrikanUtils.Utilities;

/// <summary>
/// Set of utilities to help re-sync asynchronous functions.
/// </summary>
public static class AsyncUtilities
{
    private static readonly ConcurrentQueue<Action> NextFrame = new();

    /// <summary>
    /// Method to ensure something is executed on the main thread and not in a separate thread.
    /// This will execute the action in the next frame.
    /// </summary>
    /// <param name="action">Action to execute on the main thread</param>
    public static void ExecuteOnMainThread(Action action)
    {
        NextFrame.Enqueue(action);
    }

    internal class AsyncUtilitiesComponent : MonoBehaviour
    {
        private void Update()
        {
            while (NextFrame.TryDequeue(out var action))
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    Logger.Error($"Caught exception from action: {e}");
                }
            }
        }
    }
}