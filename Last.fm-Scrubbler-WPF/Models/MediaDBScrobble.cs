using System;

namespace Scrubbler.Models
{
  /// <summary>
  /// Scrobble parsed from a media player library.
  /// </summary>
  public class MediaDBScrobble : DatedScrobble
  {
    #region Properties

    /// <summary>
    /// Number of times this track has been played.
    /// </summary>
    public int PlayCount
    {
      get { return _playCount; }
      private set
      {
        if (value <= 0)
          throw new Exception("PlayCount can't <= 0.");

        _playCount = value;
      }
    }
    private int _playCount;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="playCount">Number of times this track has been played.</param>
    /// <param name="played">Time the track was played / scrobbled.</param>
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="albumArtist">Name of the album artist.</param>
    /// <param name="duration">Length of this track.</param>
    public MediaDBScrobble(int playCount, DateTime played, string trackName, string artistName, string albumName = "", string albumArtist = "", TimeSpan? duration = null)
      : base(played, trackName, artistName, albumName, albumArtist, duration)
    {
      PlayCount = playCount;
    }
  }
}