using System.IO;

namespace Scrubbler.Helper
{
  /// <summary>
  /// Operates with directories on the storage medium.
  /// </summary>
  class DirectoryOperator : IDirectoryOperator
  {
    /// <summary>
    /// Creates all directories and subdirectories in the specified
    /// path unless they already exist.
    /// </summary>
    /// <param name="path">The directory to create.</param>
    /// <returns>An object that represents the directory at the specified path.
    /// This object is returned regardless of whether a directory at the
    /// specified path already exists.</returns>
    public object CreateDirectory(string path)
    {
      return Directory.CreateDirectory(path);
    }

    /// <summary>
    /// Determines whether the given path refers to an existing directory on disk.
    /// </summary>
    /// <param name="path">The path to test.</param>
    /// <returns>True if path refers to an existing directory; false if the directory does not
    /// exist or an error occurs when trying to determine if the specified file exists.</returns>
    public bool Exists(string path)
    {
      return Directory.Exists(path);
    }

    /// <summary>
    /// Returns the names of files (including their paths) in the specified directory.
    /// </summary>
    /// <param name="path">The relative or absolute path to the directory to search.
    /// This string is not case-sensitive.</param>
    /// <returns>An array of the full names (including paths) for the files in the specified
    /// directory, or an empty array if no files are found.</returns>
    public string[] GetFiles(string path)
    {
      return Directory.GetFiles(path);
    }
  }
}