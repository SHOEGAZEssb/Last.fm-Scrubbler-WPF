using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF
{
  /// <summary>
  /// Extension methods for <see cref="Task"/>s.
  /// </summary>
  static class TastExtensions
  {
    /// <summary>
    /// Explicitly states that we don't want to
    /// do anything with the result of a task.
    /// </summary>
    /// <param name="t">Task whose result to forget.</param>
    public static void Forget(this Task t)
    { }
  }
}