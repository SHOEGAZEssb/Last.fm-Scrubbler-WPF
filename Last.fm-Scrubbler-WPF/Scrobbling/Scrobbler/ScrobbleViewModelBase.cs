using IF.Lastfm.Core.Objects;
using Scrubbler.Helper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Scrubbler.Scrobbling.Scrobbler
{
  /// <summary>
  /// Base class for all scrobbler ViewModels.
  /// </summary>
  public abstract class ScrobbleViewModelBase : TabViewModel
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

    /// <summary>
    /// Command for scrobbling.
    /// </summary>
    public ICommand ScrobbleCommand { get; }

    /// <summary>
    /// Command for previewing the scrobbles.
    /// </summary>
    public ICommand PreviewCommand { get; }

    #endregion Properties

    #region Member

    /// <summary>
    /// WindowManager used to display dialogs.
    /// </summary>
    protected IExtendedWindowManager WindowManager;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="displayName">Display name.</param>
    protected ScrobbleViewModelBase(IExtendedWindowManager windowManager, string displayName)
      : base(displayName)
    {
      WindowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
      ScrobbleCommand = new DelegateCommand((o) => Scrobble().Forget());
      PreviewCommand = new DelegateCommand((o) => Preview());
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
      WindowManager.ShowDialog(new ScrobblePreviewViewModel(CreateScrobbles()));
    }
  }
}