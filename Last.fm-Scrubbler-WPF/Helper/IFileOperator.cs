﻿using System.IO;

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
    /// Opens a text file, reads all lines of the file,
    /// and then closes the file.
    /// </summary>
    /// <param name="path">The file to read.</param>
    /// <returns>The read lines.</returns>
    string[] ReadAllLines(string path);

    /// <summary>
    /// Opens a System.IO.FileStream on the specified path
    /// with read/write access with no sharing.
    /// </summary>
    /// <param name="path">The file to open.</param>
    /// <param name="mode">Mode to open the file in.</param>
    /// <returns>Stream of the opened file.</returns>
    FileStream Open(string path, FileMode mode);

    /// <summary>
    /// Deletes the specified file.
    /// </summary>
    /// <param name="path">The name of the file to be deleted.
    /// Wildcard characters are not supported.</param>
    void Delete(string path);

    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="path">The file to check.</param>
    /// <returns>True if the file already exists, otherwise false.</returns>
    bool Exists(string path);
  }
}