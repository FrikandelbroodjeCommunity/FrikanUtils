using System;
using System.Linq;
using System.Reflection;
using CustomPlayerEffects;
using InventorySystem.Items.Usables.Scp244.Hypothermia;
using LabApi.Features.Wrappers;

namespace FrikanUtils.Utilities;

/// <summary>
/// Utilities to help with using effects.
/// </summary>
public static class EffectUtilities
{
    private static readonly Random Random = new();

    private static readonly Type[] PositiveEffects =
    [
        typeof(Scp1853), typeof(Invigorated), typeof(Invisible), typeof(RainbowTaste), typeof(BodyshotReduction),
        typeof(DamageReduction), typeof(MovementBoost), typeof(Vitality), typeof(SpawnProtected), typeof(Ghostly),
        typeof(SilentWalk), typeof(Fade)
    ];

    private static readonly Type[] MixedEffects =
    [
        typeof(Scp1344), typeof(Scp207)
    ];

    private static readonly Type[] NegativeEffects =
    [
        typeof(AmnesiaVision), typeof(AmnesiaItems), typeof(Asphyxiated), typeof(Bleeding), typeof(Blurred),
        typeof(Burned), typeof(Concussed), typeof(Corroding), typeof(PocketCorroding), typeof(Deafened),
        typeof(Decontaminating), typeof(Disabled), typeof(Ensnared), typeof(Exhausted), typeof(Flashed),
        typeof(Hemorrhage), typeof(Hypothermia), typeof(Poisoned), typeof(Sinkhole), typeof(Stained),
        typeof(SeveredHands), typeof(Traumatized), typeof(CardiacArrest), typeof(Strangled), typeof(Slowness),
        typeof(Blindness), typeof(SeveredEyes), typeof(PitDeath)
    ];

    private static readonly Type[] TechnicalEffects =
    [
        typeof(SoundtrackMute), typeof(Scanned), typeof(FogControl), typeof(Scp1576), typeof(Lightweight),
        typeof(HeavyFooted), typeof(Scp1509Resurrected), typeof(NightVision)
    ];

    private static readonly MethodInfo Method = typeof(Player).GetMethods()
        .First(x => x.Name == nameof(Player.EnableEffect) && x.IsGenericMethod);

    /// <summary>
    /// Grant a random effect to the player based on the given criteria.
    /// </summary>
    /// <param name="player">Player to give the effect to</param>
    /// <param name="effects">The type of effects that may be given</param>
    /// <param name="duration">The duration the effect should be applied for, use 0 for permanent</param>
    /// <param name="intensity">Intensity of the effect, use -1 for a random intensity</param>
    /// <returns>The enabled status effect</returns>
    public static StatusEffectBase EnableRandomEffect(this Player player,
        EffectClassificationFlag effects = EffectClassificationFlag.All, int duration = 0, int intensity = 1)
    {
        var count = 0;
        if (effects.HasFlag(EffectClassificationFlag.Technical))
        {
            count += TechnicalEffects.Length;
        }

        if (effects.HasFlag(EffectClassificationFlag.Negative))
        {
            count += NegativeEffects.Length;
        }

        if (effects.HasFlag(EffectClassificationFlag.Mixed))
        {
            count += MixedEffects.Length;
        }

        if (effects.HasFlag(EffectClassificationFlag.Positive))
        {
            count += PositiveEffects.Length;
        }

        if (count == 0)
        {
            return null;
        }

        if (intensity < 0)
        {
            intensity = Random.Next(byte.MinValue, byte.MaxValue + 1);
        }

        var randomNumber = Random.Next(count);
        var actualIntensity = (byte)intensity;

        if (effects.HasFlag(EffectClassificationFlag.Technical))
        {
            if (randomNumber < TechnicalEffects.Length)
            {
                return (StatusEffectBase)Method.MakeGenericMethod(TechnicalEffects[randomNumber])
                    .Invoke(player, [duration, actualIntensity, false]);
            }

            randomNumber -= TechnicalEffects.Length;
        }

        if (effects.HasFlag(EffectClassificationFlag.Negative))
        {
            if (randomNumber < NegativeEffects.Length)
            {
                return (StatusEffectBase)Method.MakeGenericMethod(NegativeEffects[randomNumber])
                    .Invoke(player, [duration, actualIntensity, false]);
            }

            randomNumber -= NegativeEffects.Length;
        }

        if (effects.HasFlag(EffectClassificationFlag.Mixed))
        {
            if (randomNumber < MixedEffects.Length)
            {
                return (StatusEffectBase)Method.MakeGenericMethod(MixedEffects[randomNumber])
                    .Invoke(player, [duration, actualIntensity, false]);
            }

            randomNumber -= MixedEffects.Length;
        }

        if (effects.HasFlag(EffectClassificationFlag.Positive))
        {
            return (StatusEffectBase)Method.MakeGenericMethod(PositiveEffects[randomNumber])
                .Invoke(player, [duration, actualIntensity, false]);
        }

        return null;
    }

    /// <summary>
    /// Flag that determines the type of effects that something should apply to.
    /// </summary>
    [Flags]
    public enum EffectClassificationFlag
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        None = 0,
        Technical = 1,
        Negative = 2,
        Mixed = 4,
        Positive = 8,
        All = Technical | Negative | Mixed | Positive
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}