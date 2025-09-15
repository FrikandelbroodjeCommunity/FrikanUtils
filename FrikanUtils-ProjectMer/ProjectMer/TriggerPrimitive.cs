using MEC;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace FrikanUtils.ProjectMer;

/// <summary>
/// Component to add to triggers. Extend this class to listen to the trigger events.
/// </summary>
public abstract class TriggerPrimitive : MonoBehaviour
{
    private void Awake()
    {
        // Make the collider a trigger and enable it
        Timing.CallDelayed(.1f, () =>
        {
            if (!TryGetComponent(out Collider collider))
            {
                Logger.Warn("Failed to create trigger. No collider found!");
                return;
            }

            collider.enabled = true;
            collider.isTrigger = true;
        });
    }
}