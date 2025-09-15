using System.Linq;
using LabApi.Features.Wrappers;
using PlayerRoles;
using UnityEngine;

namespace FrikanUtils.HintSystem;

public class HintSender : MonoBehaviour
{
    private int _color;
    private float _time = 1f;

    private void Update()
    {
        _time -= Time.deltaTime;

        if (_time > 0) return;
        if (Player.Count == 0)
        {
            _time = 5f;
            return;
        }

        _time = 1f;

        var color = UtilitiesPlugin.PluginConfig.RainbowTextColors[_color];
        var duration = UtilitiesPlugin.PluginConfig.HintRefreshTime * 1.5f;
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

        _color++;
        _color %= UtilitiesPlugin.PluginConfig.RainbowTextColors.Length;
    }
}