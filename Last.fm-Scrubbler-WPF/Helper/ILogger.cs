namespace Scrubbler.Helper
{
  /// <summary>
  /// Interface for an object that logs messages.
  /// </summary>
  public interface ILogger
  {
    /// <summary>
    /// Writes the given <paramref name="text"/> to the log file.
    /// </summary>
    /// <param name="text">Text to log.</param>
    void Log(string text);

    /// <summary>
    /// If true, a timestamp will be added
    /// before the log message.
    /// </summary>
    bool AddTimestamp { get; set; }
  }
}