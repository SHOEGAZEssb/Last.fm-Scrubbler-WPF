using IF.Lastfm.Core.Objects;
using Scrubbler.Interfaces;
using Scrubbler.ViewModels.SubViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scrubbler.ViewModels.ScrobbleViewModels
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
    public virtual bool CanScrobble => Scrobbler?.IsAuthenticated ?? false;

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public abstract bool CanPreview { get; }

    /// <summary>
    /// Scrobbler used to scrobble.
    /// </summary>
    public IUserScrobbler Scrobbler
    {
      get { return _scrobbler; }
      set
      {
        _scrobbler = value;
        NotifyOfPropertyChange();
        NotifyOfPropertyChange(() => CanScrobble);
      }
    }
    private IUserScrobbler _scrobbler;

    #endregion Properties

    #region Member

    /// <summary>
    /// WindowManager used to display dialogs.
    /// </summary>
    protected IExtendedWindowManager _windowManager;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="displayName">Display name.</param>
    public ScrobbleViewModelBase(IExtendedWindowManager windowManager, string displayName)
    {
      DisplayName = displayName;
      _windowManager = windowManager;
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
      _windowManager.ShowDialog(new ScrobblePreviewViewModel(CreateScrobbles()));
    }
  }
}