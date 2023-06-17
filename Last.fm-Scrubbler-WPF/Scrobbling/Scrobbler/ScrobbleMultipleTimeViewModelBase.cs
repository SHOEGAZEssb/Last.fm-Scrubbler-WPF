using Scrubbler.Helper;
using Scrubbler.Scrobbling.Data;

namespace Scrubbler.Scrobbling.Scrobbler
{
  /// <summary>
  /// Base class for ViewModels that scrobble multiple scrobbles
  /// with selectable time.
  /// </summary>
  /// <typeparam name="T">Type of the scrobble.</typeparam>
  public abstract class ScrobbleMultipleTimeViewModelBase<T> : ScrobbleMultipleViewModelBase<T> where T : IScrobbableObjectViewModel
  {
    #region Properties

    /// <summary>
    /// ViewModel for selecting the time to scrobble.
    /// </summary>
    public ScrobbleTimeViewModel ScrobbleTimeVM
    {
      get { return _scrobbleTimeVM; }
      private set
      {
        _scrobbleTimeVM = value;
        NotifyOfPropertyChange();
      }
    }
    private ScrobbleTimeViewModel _scrobbleTimeVM;

    /// <summary>
    /// Gets if the scrobble button on the ui is enabled.
    /// </summary>
    public override bool CanScrobble => base.CanScrobble && ScrobbleTimeVM.IsTimeValid();

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="displayName">Display name.</param>
    public ScrobbleMultipleTimeViewModelBase(IExtendedWindowManager windowManager, string displayName)
      : base(windowManager, displayName)
    {
      ScrobbleTimeVM = new ScrobbleTimeViewModel();
      ScrobbleTimeVM.TimeChanged += ScrobbleTimeVM_TimeChanged;
    }

    #endregion Construction

    private void ScrobbleTimeVM_TimeChanged(object sender, System.EventArgs e)
    {
      NotifyOfPropertyChange(() => CanScrobble);
    }
  }
}