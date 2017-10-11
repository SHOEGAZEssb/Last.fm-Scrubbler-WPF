using Caliburn.Micro;
using Last.fm_Scrubbler_WPF.Models;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels.SubViewModels
{
  /// <summary>
  /// ViewModel for a <see cref="DatedScrobble"/> that has been
  /// fetched, parsed or gotten from somewhere else.
  /// </summary>
  class DatedScrobbleViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Event that triggers when <see cref="ToScrobble"/> changed.
    /// </summary>
    public event EventHandler ToScrobbleChanged;

    /// <summary>
    /// The scrobble.
    /// </summary>
    public DatedScrobble Scrobble
    {
      get { return _parsedScrobble; }
      private set
      {
        _parsedScrobble = value;
        NotifyOfPropertyChange(() => Scrobble);
        NotifyOfPropertyChange(() => IsEnabled);
      }
    }
    private DatedScrobble _parsedScrobble;

    /// <summary>
    /// Indicates if this scrobble should be scrobbled.
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
    /// Gets if the "Scrobble?" CheckBox is enabled.
    /// </summary>
    public bool IsEnabled
    {
      get { return Scrobble.Played > DateTime.Now.Subtract(TimeSpan.FromDays(14)); }
    }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="scrobble">The scrobble.</param>
    public DatedScrobbleViewModel(DatedScrobble scrobble)
    {
      Scrobble = scrobble;
    }
  }
}