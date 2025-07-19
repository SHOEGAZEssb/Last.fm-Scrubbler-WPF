using System.Diagnostics;

namespace ScrubblerLib.Helper
{
  /// <summary>
  /// Abstraction object for working with process functions.
  /// </summary>
  public class ProcessManager : IProcessManager
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