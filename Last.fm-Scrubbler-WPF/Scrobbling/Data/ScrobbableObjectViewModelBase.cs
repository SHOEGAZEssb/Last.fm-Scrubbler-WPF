using System;

namespace Scrubbler.Scrobbling.Data
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
      get => _toScrobble;
      set
      {
        if (ToScrobble != value)
        {
          _toScrobble = value;
          NotifyOfPropertyChange();
          ToScrobbleChanged?.Invoke(this, EventArgs.Empty);
        }
      }
    }
    private bool _toScrobble;

    /// <summary>
    /// If this object is selected in the UI.
    /// </summary>
    public bool IsSelected
    {
      get => _isSelected;
      set
      {
        if (IsSelected != value)
        {
          _isSelected = value;
          NotifyOfPropertyChange();
          IsSelectedChanged?.Invoke(this, EventArgs.Empty);
        }
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

    /// <summary>
    /// Updates the value of <see cref="ToScrobble"/>
    /// without notifying.
    /// </summary>
    /// <param name="toScrobble">ToScrobble value.</param>
    public void UpdateToScrobbleSilent(bool toScrobble)
    {
      _toScrobble = toScrobble;
    }

    /// <summary>
    /// Updates the value of <see cref="IsSelected"/>
    /// without notifying.
    /// </summary>
    /// <param name="isSelected">IsSelected value.</param>
    public void UpdateIsSelectedSilent(bool isSelected)
    {
      _isSelected = isSelected;
    }
  }
}