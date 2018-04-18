using Caliburn.Micro;
using System;

namespace Scrubbler.ViewModels
{
  /// <summary>
  /// Base class for all ViewModels.
  /// </summary>
  public abstract class ViewModelBase : Screen
  {
    #region Properties

    /// <summary>
    /// Event that triggers when the status should be changed.
    /// </summary>
    public event EventHandler<UpdateStatusEventArgs> StatusUpdated;

    /// <summary>
    /// Gets if certain controls that modify the
    /// scrobbling data are enabled.
    /// </summary>
    public abstract bool EnableControls { get; protected set; }

    /// <summary>
    /// Gets if certain controls that modify the
    /// scrobbling data are enabled.
    /// </summary>
    protected bool _enableControls;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    public ViewModelBase()
    {
      EnableControls = true;
    }

    /// <summary>
    /// Updates the status.
    /// </summary>
    /// <param name="newStatus">New status.</param>
    protected virtual void OnStatusUpdated(string newStatus)
    {
      StatusUpdated?.Invoke(this, new UpdateStatusEventArgs(newStatus));
    }
  }
}