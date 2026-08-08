using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LabApi.Loader.Constants;
using LabApi.Loader.Features.Yaml;
using MapGeneration.Holidays;
using Utf8Json;

namespace FrikanUtils.FileSystem.Providers;

/// <summary>
/// Represents a file provider used by the <see cref="FileHandler"/>.
/// A default implementation can be found in <see cref="LocalFileProvider"/>.
///
/// It allows for asynchronous functions, this way a provider has full control over how files are found.
/// </summary>
public abstract class BaseFileProvider : IEquatable<BaseFileProvider>, IComparable<BaseFileProvider>
{
    /// <summary>
    /// The name of the file provider, used for logging and making sure the provider is only registered once.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Sets the priority for the file provider. A lower priority will be attempted first.
    /// The default is <see cref="Priority.Medium"/>.
    ///
    /// See also: <see cref="Priority"/>
    /// </summary>
    public virtual byte LoadPriority => Priority.Medium;
    
    /// <summary>
    /// Search for the full path on the disk for a target file.
    /// This is an async method, allowing files to be downloaded and written to the drive during execution.
    /// </summary>
    /// <param name="filename">The name of the file</param>
    /// <param name="folder">The folder the file should be in</param>
    /// <returns>The full path to the file or <c>null</c></returns>
    public abstract Task<string> SearchFullPath(string filename, string folder);

    /// <summary>
    /// Search for the file and read its contents.
    /// Will return <c>null</c> if the file was not found.
    /// This is an async method, allowing files to be downloaded.
    /// </summary>
    /// <param name="filename">The name of the file</param>
    /// <param name="folder">The folder the file should be in</param>
    /// <returns>The contents of the file, or <c>null</c></returns>
    public virtual async Task<string> SearchFileContents(string filename, string folder)
    {
        var path = await SearchFullPath(filename, folder);
        return string.IsNullOrEmpty(path) ? null : File.ReadAllText(path);
    }

    /// <summary>
    /// Search for the file and convert it into the data as needed.
    /// Will return <c>null</c> if the file was not found, or it could not be parsed.
    /// This is an async method, allowing files to be downloaded and then parsed during execution.
    /// </summary>
    /// <param name="filename">The name of the file</param>
    /// <param name="folder">The folder the file should be in</param>
    /// <param name="json">Whether to read it as JSON or YAML</param>
    /// <typeparam name="T">The type the contents should be parsed to</typeparam>
    /// <returns>The file contents as <c>T</c>, or <c>null</c></returns>
    public virtual async Task<T> SearchFile<T>(string filename, string folder, bool json) where T : class
    {
        var contents = await SearchFileContents(filename, folder);
        if (string.IsNullOrEmpty(contents))
        {
            return null;
        }

        return json
            ? JsonSerializer.Deserialize<T>(contents)
            : YamlConfigParser.Deserializer.Deserialize<T>(contents);
    }

    /// <summary>
    /// Helper method to get all holiday variants of a filename.
    /// </summary>
    /// <param name="filename">The original filename</param>
    /// <returns>All holiday filenames</returns>
    protected static IEnumerable<string> GetHolidayFilenames(string filename)
        => GetHolidayFilenames(filename, null);

    /// <summary>
    /// Helper method to get all holiday variants of a filename.
    /// </summary>
    /// <param name="filename">The original filename</param>
    /// <param name="allowedTypes">The holidays that are allowed to be detected. When given null, it will allow all holiday types.</param>
    /// <returns>All holiday filenames</returns>
    protected static IEnumerable<string> GetHolidayFilenames(string filename, HolidayType[] allowedTypes)
    {
        foreach (HolidayType type in Enum.GetValues(typeof(HolidayType)))
        {
            if (!HolidayUtils.IsHolidayActive(type) || type == HolidayType.None || !allowedTypes.Contains(type)) continue;
            yield return $"{type}-{filename}";
        }

        yield return filename;
    }

    /// <inheritdoc/>
    public bool Equals(BaseFileProvider other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((BaseFileProvider)obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Name != null ? Name.GetHashCode() : 0;
    }
    
    public int CompareTo(BaseFileProvider other)
    {
        if (Equals(other)) return 0;
        return other is null ? 1 : LoadPriority.CompareTo(other.LoadPriority);
    }
}