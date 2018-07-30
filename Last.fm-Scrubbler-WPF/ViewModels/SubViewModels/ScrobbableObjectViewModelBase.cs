using Caliburn.Micro;
using Scrubbler.Interfaces;
using Scrubbler.Models;
using System;

namespace Scrubbler.ViewModels.SubViewModels
{
  /// <summary>
  /// Base ViewModel for all ViewModels that
  /// manage scrobbable objects.
  /// </summary>
  public abstract class ScrobbableObjectViewModelBase : ScrobbleViewModel, IScrobbableObjectViewModel
  {
    #region Properties

    /// <summary>
    /// Event that triggers when <see cref="ToScrobble"/> changed.
    /// </summary>
    public event EventHandler ToScrobbleChanged;

    /// <summary>
    /// Event that triggers when <see cref="IsSelected"/> changes.
    /// </summary>
    public event EventHandler IsSelectedChanged;

    /// <summary>
    /// If this object should be scrobbled.
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

    /// <summary>
    /// If this object is selected in the UI.
    /// </summary>
    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        _isSelected = value;
        NotifyOfPropertyChange();
        IsSelectedChanged?.Invoke(this, EventArgs.Empty);
      }
    }
    private bool _isSelected;

    #endregion Properties

    #region Construction

    /// <summary>
    /// The actual scrobble.
    /// </summary>
    /// <param name="scrobble"></param>
    public ScrobbableObjectViewModelBase(ScrobbleBase scrobble)
      : base(scrobble)
    { }

    #endregion Construction
  }
}