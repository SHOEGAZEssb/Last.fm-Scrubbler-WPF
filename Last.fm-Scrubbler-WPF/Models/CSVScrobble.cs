using System;

namespace Last.fm_Scrubbler_WPF.Models
{
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
    /// <param name="dateTime">Time of the scrobble.</param>
    public CSVScrobble(string artist, string album, string track, DateTime dateTime)
    {
      Artist = artist;
      Album = album;
      Track = track;
      DateTime = dateTime;
    }
  }
}