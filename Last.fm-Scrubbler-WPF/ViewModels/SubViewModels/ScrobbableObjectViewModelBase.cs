using Caliburn.Micro;
using System;

namespace Scrubbler.ViewModels.SubViewModels
{
  /// <summary>
  /// Base ViewModel for all ViewModels that
  /// manage scrobbable objects.
  /// </summary>
  public abstract class ScrobbableObjectViewModelBase : Screen
  {
    #region Properties

    /// <summary>
    /// Event that triggers when <see cref="ToScrobble"/> changed.
    /// </summary>
    public event EventHandler ToScrobbleChanged;

    /// <summary>
    /// if this object should be scrobbled.
    /// </summary>
    public bool ToScrobble
    {
      get { return _toScrobble; }
      set
      {
        _toScrobble = value;
        NotifyOfPropertyChange(() => ToScrobble);
        ToScrobbleChanged?.Invoke(this, EventArgs.Empty);
      }
    }
    private bool _toScrobble;

    #endregion Properties
  }
}