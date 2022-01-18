using System.IO;

namespace Scrubbler.Helper
{
  /// <summary>
  /// Interfaces with files.
  /// </summary>
  class FileOperator : IFileOperator
  {
    /// <summary>
    /// Deletes the specified file.
    /// </summary>
    /// <param name="path">The name of the file to be deleted.
    /// Wildcard characters are not supported.</param>
    public void Delete(string path)
    {
      File.Delete(path);
    }

    /// <summary>
    /// Creates a new file, writes the specified string to the file, and then closes
    /// the file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="path">The file to write to.</param>
    /// <param name="contents">The string to write to the file.</param>
    public void WriteAllLines(string path, string[] contents)
    {
      File.WriteAllLines(path, contents);
    }

    /// <summary>
    /// Creates a new file, writes the specified string to the file, and then closes
    /// the file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="path">The file to write to.</param>
    /// <param name="contents">The string to write to the file.</param>
    public void WriteAllText(string path, string contents)
    {
      File.WriteAllText(path, contents);
    }

    /// <summary>
    /// Opens a text file, reads all lines of the file,
    /// and then closes the file.
    /// </summary>
    /// <param name="path">The file to read.</param>
    /// <returns>The read lines.</returns>
    public string[] ReadAllLines(string path)
    {
      return File.ReadAllLines(path);
    }

    /// <summary>
    /// Opens a System.IO.FileStream on the specified path
    /// with read/write access with no sharing.
    /// </summary>
    /// <param name="path">The file to open.</param>
    /// <param name="mode">Mode to open the file in.</param>
    /// <returns>Stream of the opened file.</returns>
    public FileStream Open(string path, FileMode mode)
    {
      return File.Open(path, mode);
    }

    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="path">The file to check.</param>
    /// <returns>True if the file already exists, otherwise false.</returns>
    public bool Exists(string path)
    {
      return File.Exists(path);
    }
  }
}