using System;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// Base class for all scrobblers.
  /// </summary>
  public abstract class ScrobbleViewModelBase : ViewModelBase
  {
    #region Properties

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
  }
}