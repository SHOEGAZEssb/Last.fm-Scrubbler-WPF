using System;

namespace Last.fm_Scrubbler_WPF.Models
{
  /// <summary>
  /// A scrobble parsed from a csv line.
  /// </summary>
  class CSVScrobble
  {
    #region Properties

    /// <summary>
    /// Name of the artist.
    /// </summary>
    public string Artist
    {
      get { return _artist; }
      private set { _artist = value; }
    }
    private string _artist;

    /// <summary>
    /// Name of the album.
    /// </summary>
    public string Album
    {
      get { return _album; }
      private set { _album = value; }
    }
    private string _album;

    /// <summary>
    /// Name of the track.
    /// </summary>
    public string Track
    {
      get { return _track; }
      private set { _track = value; }
    }
    private string _track;

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
    /// Length of the track.
    /// </summary>
    public TimeSpan Duration
    {
      get { return _duration; }
      private set { _duration = value; }
    }
    private TimeSpan _duration;

    /// <summary>
    /// Time scrobbled.
    /// </summary>
    public DateTime DateTime
    {
      get { return _dateTime; }
      private set { _dateTime = value; }
    }
    private DateTime _dateTime;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="artist">Name of the artist.</param>
    /// <param name="album">Name of the album.</param>
    /// <param name="track">Name of the track.</param>
    /// <param name="albumArtist">Name of the album artist.</param>
    /// <param name="duration">Length of the track.</param>
    /// <param name="dateTime">Time of the scrobble.</param>
    public CSVScrobble(string artist, string album, string track, string albumArtist, TimeSpan duration, DateTime dateTime)
    {
      Artist = artist;
      Album = album;
      Track = track;
      AlbumArtist = albumArtist;
      Duration = duration;
      DateTime = dateTime;
    }
  }
}