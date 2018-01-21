using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Interfaces;
using System;
using System.Collections.Generic;
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

    #region Member

    /// <summary>
    /// WindowManager used to display dialogs.
    /// </summary>
    protected IWindowManager _windowManager;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="scrobbler">Scrobbler used to scrobble.</param>
    public ScrobbleViewModelBase(IWindowManager windowManager, IAuthScrobbler scrobbler)
    {
      _windowManager = windowManager;
      Scrobbler = scrobbler;
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
    /// Creates a list with scrobbles that will be scrobbled
    /// with the current configuration.
    /// </summary>
    /// <returns>List with scrobbles.</returns>
    protected abstract IEnumerable<Scrobble> CreateScrobbles();

    /// <summary>
    /// Shows a preview of the tracks that will be scrobbled.
    /// </summary>
    public virtual void Preview()
    {
      _windowManager.ShowWindow(new ScrobblePreviewViewModel(CreateScrobbles()));
    }
  }
}