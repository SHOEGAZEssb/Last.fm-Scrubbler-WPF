namespace Scrubbler.Helper
{
  /// <summary>
  /// Interface for an object interfacing with files
  /// on the storage medium.
  /// </summary>
  public interface IFileOperator
  {
    /// <summary>
    /// Creates a new file, writes the specified string to the file, and then closes
    /// the file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="path">The file to write to.</param>
    /// <param name="contents">The string to write to the file.</param>
    void WriteAllText(string path, string contents);

    /// <summary>
    /// Creates a new file, write the specified string array to the file, and then closes
    /// the file.
    /// </summary>
    /// <param name="path">The file to write to.</param>
    /// <param name="contents">The string array to write to the file.</param>
    void WriteAllLines(string path, string[] contents);

    /// <summary>
    /// Deletes the specified file.
    /// </summary>
    /// <param name="path">The name of the file to be deleted.
    /// Wildcard characters are not supported.</param>
    void Delete(string path);
  }
}