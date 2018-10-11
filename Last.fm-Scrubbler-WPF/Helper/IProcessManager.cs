using System.Diagnostics;

namespace Scrubbler.Helper
{
  /// <summary>
  /// Interface for an object managing process functions.
  /// </summary>
  public interface IProcessManager
  {
    /// <summary>
    /// Starts a process for the given <paramref name="fileName"/>.
    /// </summary>
    /// <param name="fileName">File name of the process to start.</param>
    /// <returns>Started process.</returns>

    Process Start(string fileName);
  }
}