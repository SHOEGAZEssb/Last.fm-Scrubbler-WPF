using System.Diagnostics;

namespace Scrubbler.Helper
{
  /// <summary>
  /// Abstraction object for working with process functions.
  /// </summary>
  class ProcessManager : IProcessManager
  {
    /// <summary>
    /// Starts a process for the given <paramref name="fileName"/>.
    /// </summary>
    /// <param name="fileName">File name of the process to start.</param>
    /// <returns>Started process.</returns>
    public Process Start(string fileName)
    {
      return Process.Start(fileName);
    }
  }
}