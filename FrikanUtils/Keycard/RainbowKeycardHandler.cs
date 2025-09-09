using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace FrikanUtils.Keycard;

public class RainbowKeycardHandler : MonoBehaviour
{
    public static RainbowKeycardHandler Instance { get; private set; }

    internal static readonly List<CustomKeycard> Keycards = [];
    private static float _hue;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        _hue += 0.1f * Time.deltaTime;
        if (_hue > 1)
        {
            _hue -= 1;
        }

        foreach (var keycard in Keycards.Where(x => x.IsHeld))
        {
            keycard.Tint = Color.HSVToRGB(_hue, 1, 1);
            keycard.Apply();
        }
    }

    /// <summary>
    /// Add keycards where the tint should be <i>rainbow</i>.
    /// </summary>
    /// <param name="card">Keycard</param>
    public static void AddRainbowKeycard(CustomKeycard card) => Keycards.AddIfNotContains(card);

    /// <summary>
    /// Remove the rainbow tint from the keycard.
    /// Can also be called to make sure the rainbow tint is not applied when updating the keycard.
    /// </summary>
    /// <param name="card">Keycard</param>
    public static void RemoveRainbowKeycard(CustomKeycard card) => Keycards.Remove(card);

    /// <summary>
    /// Toggle the rainbow tint on/off for the keycard.
    /// </summary>
    /// <param name="card">Keycard</param>
    public static void ToggleRainbowKeycard(CustomKeycard card)
    {
        if (Keycards.Remove(card))
        {
            return;
        }

        Keycards.AddIfNotContains(card);
    }
}