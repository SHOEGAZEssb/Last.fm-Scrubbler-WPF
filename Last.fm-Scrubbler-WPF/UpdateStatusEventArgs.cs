using System;

namespace Scrubbler
{
  /// <summary>
  /// EventArgs that contain a new status for the status bar.
  /// </summary>
  public class UpdateStatusEventArgs : EventArgs
  {
    /// <summary>
    /// The new status string.
    /// </summary>
    public string NewStatus { get; private set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="newStatus">The new status string.</param>
    public UpdateStatusEventArgs(string newStatus)
    {
      NewStatus = newStatus;
    }
  }
}