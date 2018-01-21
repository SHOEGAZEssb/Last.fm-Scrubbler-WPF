using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for a single fetched track of a friend.
  /// Used in the <see cref="FriendScrobbleViewModel"/>.
  /// </summary>
  public class FetchedFriendTrackViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Event that triggers when <see cref="ToScrobble"/> changes.
    /// </summary>
    public event EventHandler ToScrobbleChanged;

    /// <summary>
    /// Gets the fetched scrobble.
    /// </summary>
    public LastTrack Track
    {
      get { return _scrobble; }
      private set
      {
        _scrobble = value;
        NotifyOfPropertyChange(() => Track);
        NotifyOfPropertyChange(() => IsEnabled);
      }
    }
    private LastTrack _scrobble;

    /// <summary>
    /// Gets/sets if this track should be scrobbled.
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
      get { return Track.TimePlayed.Value.LocalDateTime > DateTime.Now.Subtract(TimeSpan.FromDays(14)); }
    }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="scrobble">The scrobbled track.</param>
    public FetchedFriendTrackViewModel(LastTrack scrobble)
    {
      Track = scrobble;
    }
  }
}