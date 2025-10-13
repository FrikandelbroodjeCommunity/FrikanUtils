using System.Linq;
using LabApi.Features.Wrappers;
using PlayerRoles;
using UnityEngine;

namespace FrikanUtils.HintSystem;

internal class HintSender : MonoBehaviour
{
    private int _color;
    private int _colorUpdate;
    private float _time = 1f;

    private void Update()
    {
        _time -= Time.deltaTime;

        // Wait for the timer to run out
        if (_time > 0) return;

        // There are no players, so idle for 5 more seconds
        if (Player.Count == 0)
        {
            _time = 5f;
            return;
        }

        // Restart the timer
        _time = UtilitiesPlugin.PluginConfig.HintRefreshTime;

        // Process the hints
        var color = UtilitiesPlugin.PluginConfig.RainbowTextColors[_color];
        var duration = _time * 1.5f;
        foreach (var player in Player.List.Where(x => x.IsPlayer && x.Role != RoleTypeId.Tutorial))
        {
            var hint = Round.IsRoundStarted
                ? HintHandler.GetGameText(player, color)
                : HintHandler.GetLobbyText(player, color);

            if (!string.IsNullOrEmpty(hint))
            {
                player.SendHint(hint, duration);
            }
        }

        // Check if we should continue to the next rainbow color
        if (--_colorUpdate <= 0)
        {
            _colorUpdate = UtilitiesPlugin.PluginConfig.RainbowColorTicks;

            _color++;
            _color %= UtilitiesPlugin.PluginConfig.RainbowTextColors.Length;
        }
    }
}