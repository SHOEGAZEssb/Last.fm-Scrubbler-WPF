using IF.Lastfm.Core.Objects;
using System;

namespace Scrubbler.Models
{
  /// <summary>
  /// A scrobble with a date that represents the
  /// time it was scrobbled or last played.
  /// </summary>
  public class DatedScrobble : ScrobbleBase
  {
    #region Properties

    /// <summary>
    /// Time played / scrobbled.
    /// </summary>
    public DateTime Played { get; private set; }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="played">Time the track was played / scrobbled.</param>
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="albumArtist">Name of the album artist.</param>
    /// <param name="duration">Length of this track.</param>
    public DatedScrobble(DateTime played, string trackName, string artistName, string albumName = "", string albumArtist = "", TimeSpan? duration = null)
      : base(trackName, artistName, albumName, albumArtist, duration)
    {
      Played = played;
    }

    /// <summary>
    /// Returns a new <see cref="Scrobble"/> with infos from
    /// this <see cref="DatedScrobble"/>.
    /// </summary>
    /// <returns>New Scrobble.</returns>
    public Scrobble ToLastFMScrobble()
    {
      return new Scrobble(ArtistName, AlbumName, TrackName, Played) { AlbumArtist = AlbumArtist, Duration = Duration };
    }
  }
}