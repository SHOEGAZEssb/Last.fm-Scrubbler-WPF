using System;

namespace Last.fm_Scrubbler_WPF.Models
{
  /// <summary>
  /// Basic information about a track / scrobble.
  /// </summary>
  class ScrobbleBase
  {
    #region Properties

    /// <summary>
    /// Name of the track.
    /// </summary>
    public string TrackName
    {
      get { return _trackName; }
      private set
      {
        if (value == string.Empty)
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
      private set
      {
        if (value == string.Empty)
          throw new ArgumentException("ArtistName can't be empty.");

        _artistName = value;
      }
    }
    private string _artistName;

    /// <summary>
    /// Name of the album.
    /// </summary>
    public string AlbumName
    {
      get { return _albumName; }
      private set { _albumName = value; }
    }
    private string _albumName;

    /// <summary>
    /// Name of the album artist.
    /// </summary>
    public string AlbumArtist
    {
      get { return _albumArtist; }
      private set { _albumArtist = value; }
    }
    private string _albumArtist;

    /// <summary>
    /// Length of this track.
    /// </summary>
    public TimeSpan? Duration
    {
      get { return _duration; }
      private set { _duration = value; }
    }
    private TimeSpan? _duration;

    #endregion Properties

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
  }
}