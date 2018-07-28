using Scrubbler.Interfaces;

namespace Scrubbler.ViewModels.ScrobbleViewModels
{
  /// <summary>
  /// Base class for ViewModels that scrobble multiple scrobbles
  /// with selectable time.
  /// </summary>
  /// <typeparam name="T"></typeparam>
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
    }

    #endregion Construction
  }
}