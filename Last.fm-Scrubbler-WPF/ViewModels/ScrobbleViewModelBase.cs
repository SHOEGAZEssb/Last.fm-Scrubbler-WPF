using Caliburn.Micro;
using System;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// Base class for all scrobblers.
  /// </summary>
  abstract class ScrobbleViewModelBase : PropertyChangedBase
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
    protected bool _enableControls;

    /// <summary>
    /// Gets if the scrobble button is enabled.
    /// </summary>
    public abstract bool CanScrobble { get; }

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public abstract bool CanPreview { get; }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    public ScrobbleViewModelBase()
    {
      MainViewModel.ClientAuthChanged += MainViewModel_ClientAuthChanged;
      EnableControls = true;
    }

    /// <summary>
    /// Notifies the UI that the client auth has changed.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void MainViewModel_ClientAuthChanged(object sender, EventArgs e)
    {
      NotifyOfPropertyChange(() => CanScrobble);
    }

    /// <summary>
    /// Scrobbles the selected tracks.
    /// </summary>
    /// <returns>Task.</returns>
    public abstract Task Scrobble();

    /// <summary>
    /// Shows a preview of the tracks that will be scrobbled.
    /// </summary>
    public abstract void Preview();

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