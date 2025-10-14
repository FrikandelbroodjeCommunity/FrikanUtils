using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using PlayerRoles;

namespace FrikanUtils.Audio;

internal static class AudioEventHandler
{
    internal static void RegisterEvents()
    {
        PlayerEvents.ChangingRole += OnRoleChange;
    }

    internal static void UnregisterEvents()
    {
        PlayerEvents.ChangingRole -= OnRoleChange;
    }

    private static void OnRoleChange(PlayerChangingRoleEventArgs ev)
    {
        if (ev.Player.IsPlayer)
        {
            if (ev.NewRole == RoleTypeId.Tutorial)
            {
                AudioSystem.RemoverUser(ev.Player.PlayerId);
            }
            else if (ev.OldRole.RoleTypeId == RoleTypeId.Tutorial)
            {
                AudioSystem.AddUser(ev.Player);
            }
        }

        // For NPCs check if it's one of the audio players, as they are not allowed to change role at round start.
        if (ev.Player.IsPlayer || ev.ChangeReason != RoleChangeReason.RoundStart) return;

        foreach (var audioPlayer in AudioSystem.AudioPlayers)
        {
            if (audioPlayer is not HubAudioPlayer hubPlayer || hubPlayer.Player != ev.Player) continue;

            ev.IsAllowed = false;
            break;
        }
    }
}