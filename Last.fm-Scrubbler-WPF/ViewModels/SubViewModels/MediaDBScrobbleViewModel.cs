using Caliburn.Micro;
using Last.fm_Scrubbler_WPF.Models;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for a <see cref="MediaDBScrobble"/>.
  /// </summary>
  class MediaDBScrobbleViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Event that triggers when <see cref="ToScrobble"/> changes.
    /// </summary>
    public event EventHandler ToScrobbleChanged;

    /// <summary>
    /// The parsed scrobble.
    /// </summary>
    public MediaDBScrobble Scrobble
    {
      get { return _scrobble; }
      private set
      {
        _scrobble = value;
        NotifyOfPropertyChange(() => Scrobble);
      }
    }
    private MediaDBScrobble _scrobble;

    /// <summary>
    /// Gets if this scrobble should be scrobbled.
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

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="scrobble">The parsed scrobble.</param>
    public MediaDBScrobbleViewModel(MediaDBScrobble scrobble)
    {
      Scrobble = scrobble;
    }
  }
}