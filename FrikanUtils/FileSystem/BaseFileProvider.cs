using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MapGeneration.Holidays;

namespace FrikanUtils.FileSystem;

public abstract class BaseFileProvider : IEquatable<BaseFileProvider>
{
    /// <summary>
    /// The name of the file provider, used for logging and making sure the provider is only registered once.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Search for the full path on the disk for a target file.
    /// This is an async method, allowing files to be downloaded and written to the drive during execution.
    ///
    /// This also searches for special "event" files. (e.g. "Halloween-{filename}")
    /// </summary>
    /// <param name="filename">The name of the file</param>
    /// <param name="folder">The folder the file should be in</param>
    /// <returns>The full path to the file or null</returns>
    public abstract Task<string> SearchFullPath(string filename, string folder);

    /// <summary>
    /// Search for the file and convert it into the data as needed.
    /// Will return <code>default</code> if the file was not found, or it could not be parsed.
    /// This is an async method, allowing files to be downloaded and then parsed during execution.
    /// 
    /// This also searches for special "event" files. (e.g. "Halloween-{filename}")
    /// </summary>
    /// <param name="filename">The name of the file</param>
    /// <param name="folder">The folder the file should be in</param>
    /// <param name="json">Whether to read it as JSON or YAML</param>
    /// <typeparam name="T">The type the contents should be parsed to</typeparam>
    /// <returns>The file contents as <code>T</code>, or <code>default</code></returns>
    public abstract Task<T> SearchFile<T>(string filename, string folder, bool json) where T : class;

    /// <summary>
    /// Helper method to get all holiday variants of a filename.
    /// </summary>
    /// <param name="filename">The original filename</param>
    /// <returns>All holiday filenames</returns>
    protected IEnumerable<string> GetHolidayFilenames(string filename)
    {
        foreach (HolidayType type in Enum.GetValues(typeof(HolidayType)))
        {
            if (!HolidayUtils.IsHolidayActive(type) || type == HolidayType.None) continue;
            yield return $"{type}-{filename}";
        }

        yield return filename;
    }

    public bool Equals(BaseFileProvider other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((BaseFileProvider)obj);
    }

    public override int GetHashCode()
    {
        return (Name != null ? Name.GetHashCode() : 0);
    }
}