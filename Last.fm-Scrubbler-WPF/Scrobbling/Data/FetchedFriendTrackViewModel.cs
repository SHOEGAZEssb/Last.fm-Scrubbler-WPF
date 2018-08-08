using IF.Lastfm.Core.Objects;
using System;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// ViewModel for a single fetched track of a friend.
  /// Used in the <see cref="Scrobbler.FriendScrobbleViewModel"/>.
  /// </summary>
  public class FetchedFriendTrackViewModel : ScrobbableObjectViewModelBase
  {
    #region Properties

    /// <summary>
    /// Gets the fetched scrobble.
    /// </summary>
    public LastTrack Track
    {
      get { return _scrobble; }
      private set
      {
        _scrobble = value;
        NotifyOfPropertyChange();
        NotifyOfPropertyChange(() => IsEnabled);
      }
    }
    private LastTrack _scrobble;

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
      : base(new ScrobbleBase(scrobble.Name, scrobble.ArtistName, scrobble.AlbumName, "", scrobble.Duration))
    {
      Track = scrobble;
    }
  }
}