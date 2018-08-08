using System;
using System.Diagnostics;
using System.IO;

namespace Scrubbler.Helper
{
  /// <summary>
  /// Logging class.
  /// </summary>
  public class Logger : ILogger, IDisposable
  {
    #region Properties

    /// <summary>
    /// If true, a timestamp will be added
    /// before the log message.
    /// </summary>
    public bool AddTimestamp { get; set; }

    #endregion Properties

    #region Member

    /// <summary>
    /// Lock anchor to prevent simultaneously writing of the log file.
    /// </summary>
    private readonly object _lockAnchor = new object();

    /// <summary>
    /// The <see cref="StreamWriter"/> used to write the text into
    /// the log file.
    /// </summary>
    private StreamWriter _streamWriter;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// Creates/opens the log file.
    /// </summary>
    public Logger(string logFilePath)
    {
      AddTimestamp = true;

      lock (_lockAnchor)
      {
        try
        {
          _streamWriter = new StreamWriter(File.Open(logFilePath, FileMode.Append, FileAccess.Write));
        }
        catch (Exception ex)
        {
          throw new IOException("Log file could not be created or opened. Error: " + ex.Message);
        }
      }
    }

    #endregion Construction

    /// <summary>
    /// Writes the given <paramref name="text"/> to the log file.
    /// </summary>
    /// <param name="text">Text to log.</param>
    public void Log(string text)
    {
      lock (_lockAnchor)
      {
        try
        {
          if (AddTimestamp)
            text = string.Format("[{0}]: {1}", DateTime.Now, text);

          _streamWriter.WriteLine(text);
          _streamWriter.Flush();
        }
        catch (Exception ex)
        {
          Debug.WriteLine("Error logging text. Error: " + ex.Message);
        }
      }
    }

    #region IDisposable

    /// <summary>
    /// Used to detect redundant <see cref="Dispose(bool)"/> calls.
    /// </summary>
    private bool disposedValue = false;

    /// <summary>
    /// Disposes resources.
    /// </summary>
    /// <param name="disposing">True if called by the user.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          if (_streamWriter != null)
          {
            _streamWriter.Close();
            _streamWriter.Dispose();
            _streamWriter = null;
          }
        }

        disposedValue = true;
      }
    }

    /// <summary>
    /// Finalizer.
    /// </summary>
    ~Logger()
    {
      Dispose(false);
    }

    /// <summary>
    /// Disposes resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable
  }
}