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
    /// Gets if this track can be scrobbled with the current configuration.
    /// </summary>
    public override bool CanScrobble => base.CanScrobble && TimePlayed > DateTime.Now.Subtract(TimeSpan.FromDays(14));

    /// <summary>
    /// The time this friend track was played.
    /// </summary>
    public DateTime TimePlayed => _track.TimePlayed.Value.LocalDateTime;

    /// <summary>
    /// The image of the track.
    /// </summary>
    public Uri Image => _track.Images.Largest;

    #endregion Properties

    #region Member

    /// <summary>
    /// The fetched scrobble.
    /// </summary>
    private readonly LastTrack _track;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="scrobble">The scrobbled track.</param>
    public FetchedFriendTrackViewModel(LastTrack scrobble)
      : base(new ScrobbleBase(scrobble.Name, scrobble.ArtistName, scrobble.AlbumName, "", scrobble.Duration))
    {
      _track = scrobble;
    }
  }
}