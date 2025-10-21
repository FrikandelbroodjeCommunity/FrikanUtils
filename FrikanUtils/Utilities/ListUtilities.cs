using System;
using System.Collections.Generic;
using System.Linq;

namespace FrikanUtils.Utilities;

/// <summary>
/// Set of utilities to help with lists.
/// </summary>
public static class ListUtilities
{
    private static readonly Random Random = new();

    /// <summary>
    /// Allows for easily getting a random element of any enumerable.
    /// Keep in mind it may consume the elements of the given enumerable.
    /// </summary>
    /// <param name="list">The enumerable to take an element from</param>
    /// <typeparam name="T">The type of the elements</typeparam>
    /// <returns>A randomly picked element or <c>default</c></returns>
    public static T GetRandomElement<T>(this IEnumerable<T> list)
    {
        var array = list.ToArray();
        return array.Length == 0 ? default : array[Random.Next(array.Length)];
    }
}