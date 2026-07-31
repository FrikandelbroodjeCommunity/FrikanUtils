using System;

namespace FrikanUtils.FileSystem;

/// <summary>
/// Enum used to determine which file types a <see cref="BaseFileProvider"/> is allowed to serve.
/// </summary>
[Flags]
public enum AllowedFileTypes
{
    /// <summary>
    /// Will allow the <see cref="FileHandler"/> to use the file provider.
    /// </summary>
    GenericFile = 0b1,
    
    /// <summary>
    /// Will allow the file provider to be used to sync configuration files.
    /// </summary>
    Config = 0b10
}