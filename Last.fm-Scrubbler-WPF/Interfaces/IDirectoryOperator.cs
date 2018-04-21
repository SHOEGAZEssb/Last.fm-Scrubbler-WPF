namespace Scrubbler.Interfaces
{
  /// <summary>
  /// Interface for an object operating with directories
  /// on the storage medium.
  /// </summary>
  public interface IDirectoryOperator
  {
    /// <summary>
    /// Determines whether the given path refers to an existing directory on disk.
    /// </summary>
    /// <param name="path">The path to test.</param>
    /// <returns>True if path refers to an existing directory; false if the directory does not
    /// exist or an error occurs when trying to determine if the specified file exists.</returns>
    bool Exists(string path);

    /// <summary>
    /// Creates all directories and subdirectories in the specified
    /// path unless they already exist.
    /// </summary>
    /// <param name="path">The directory to create.</param>
    /// <returns>An object that represents the directory at the specified path.
    /// This object is returned regardless of whether a directory at the
    /// specified path already exists.</returns>
    object CreateDirectory(string path);
  }
}