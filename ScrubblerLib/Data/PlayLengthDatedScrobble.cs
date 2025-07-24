using Newtonsoft.Json;
using System;

namespace ScrubblerLib.Data
{
  /// <summary>
  /// Scrobble that knows for how long it was played.
  /// </summary>
  public class PlayLengthDatedScrobble : DatedScrobble
  {
    #region Properties

    /// <summary>
    /// Number of milliseconds this track was played.
    /// </summary>
    public int? MillisecondsPlayed { get; }

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="played">Time the track was played / scrobbled.</param>
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="albumArtist">Name of the album artist.</param>
    /// <param name="duration">Length of this track.</param>
    /// <param name="millisecondsPlayed">Number of milliseconds this track was played.</param>
    [JsonConstructor]
    public PlayLengthDatedScrobble(DateTime played, string trackName, string artistName, string albumName = "", string albumArtist = "", TimeSpan? duration = null, int? millisecondsPlayed = int.MaxValue)
      : base(played, trackName, artistName, albumName, albumArtist, duration)
    {
      MillisecondsPlayed = millisecondsPlayed;
    }

    #endregion Construction
  }
}