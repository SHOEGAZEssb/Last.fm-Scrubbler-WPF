using System;

namespace Last.fm_Scrubbler_WPF.Models
{
  /// <summary>
  /// A scrobble with a date that represents the
  /// time it was scrobbled or last played.
  /// </summary>
  class DatedScrobble : ScrobbleBase
  {
    #region Properties

    /// <summary>
    /// Time played / scrobbled.
    /// </summary>
    public DateTime Played
    {
      get { return _played; }
      private set { _played = value; }
    }
    private DateTime _played;

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
  }
}