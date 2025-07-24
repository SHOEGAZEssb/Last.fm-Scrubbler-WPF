using IF.Lastfm.Core.Objects;
using System;

namespace ScrubblerLib.Data
{
  /// <summary>
  /// Basic information about a track / scrobble.
  /// </summary>
  public class ScrobbleBase
  {
    #region Properties

    /// <summary>
    /// Name of the track.
    /// </summary>
    public string TrackName
    {
      get { return _trackName; }
      set
      {
        if (string.IsNullOrEmpty(value))
          throw new ArgumentException("TrackName can't be empty.");

        _trackName = value;
      }
    }
    private string _trackName;

    /// <summary>
    /// Name of the artist.
    /// </summary>
    public string ArtistName
    {
      get { return _artistName; }
      set
      {
        if (string.IsNullOrEmpty(value))
          throw new ArgumentException("ArtistName can't be empty.");

        _artistName = value;
      }
    }
    private string _artistName;

    /// <summary>
    /// Name of the album.
    /// </summary>
    public string AlbumName { get; set; }

    /// <summary>
    /// Name of the album artist.
    /// </summary>
    public string AlbumArtist { get; set; }

    /// <summary>
    /// Length of this track.
    /// </summary>
    public TimeSpan? Duration { get; private set; }

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="albumArtist">Name of the album artist.</param>
    /// <param name="duration">Length of this track.</param>
    public ScrobbleBase(string trackName, string artistName, string albumName = "", string albumArtist = "", TimeSpan? duration = null)
    {
      TrackName = trackName;
      ArtistName = artistName;
      AlbumName = albumName;
      AlbumArtist = albumArtist;
      Duration = duration;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="scrobble">Scrobble to pull info from.</param>
    public ScrobbleBase(Scrobble scrobble)
      : this(scrobble.Track, scrobble.Artist, scrobble.Album, scrobble.AlbumArtist, scrobble.Duration)
    { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="track">Track to pull info from.</param>
    public ScrobbleBase(Track track)
      : this(track.Name, track.ArtistName, track.AlbumName, null)
    { }

    #endregion Construction
  }
}