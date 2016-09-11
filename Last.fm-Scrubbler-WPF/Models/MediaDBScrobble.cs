using System;

namespace Last.fm_Scrubbler_WPF.Models
{
  /// <summary>
  /// Scrobble parsed from a media player library.
  /// </summary>
  class MediaDBScrobble
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
        if (value == "")
          throw new Exception("TrackName can't be empty.");

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
        if (value == "")
          throw new Exception("ArtistName can't be empty.");

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
    /// <param name="trackName">Name of the track.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="albumName">Name of the album.</param>
    /// <param name="playCount">Number of times this track has been played.</param>
    public MediaDBScrobble(string trackName, string artistName, string albumName, int playCount)
    {
      TrackName = trackName;
      ArtistName = artistName;
      AlbumName = albumName;
      PlayCount = playCount;
    }
  }
}