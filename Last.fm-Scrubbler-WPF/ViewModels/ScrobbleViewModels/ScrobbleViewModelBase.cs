using IF.Lastfm.Core.Scrobblers;
using Last.fm_Scrubbler_WPF.Interfaces;
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
    public virtual bool CanScrobble
    {
      get
      {
        if (Scrobbler != null && Scrobbler.Auth != null)
          return Scrobbler.Auth.Authenticated;
        return false;
      }
    }

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public abstract bool CanPreview { get; }

    /// <summary>
    /// Scrobbler used to scrobble.
    /// </summary>
    public IAuthScrobbler Scrobbler
    {
      get { return _scrobbler; }
      set
      {
        _scrobbler = value;
        NotifyOfPropertyChange();
        NotifyOfPropertyChange(() => CanScrobble);
      }
    }
    private IAuthScrobbler _scrobbler;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="scrobbler">Scrobbler used to scrobble.</param>
    public ScrobbleViewModelBase(IAuthScrobbler scrobbler)
    {
      Scrobbler = scrobbler;
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