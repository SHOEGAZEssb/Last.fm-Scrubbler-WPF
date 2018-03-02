using Last.fm_Scrubbler_WPF.Interfaces;
using System;
using TagLib;

namespace Last.fm_Scrubbler_WPF.Models
{
  class LocalFile : ILocalFile
  {
    #region Properties

    /// <summary>
    /// The artist name.
    /// </summary>
    public string Artist => _file.Tag.FirstPerformer;

    /// <summary>
    /// The album name.
    /// </summary>
    public string Album => _file.Tag.Album;

    /// <summary>
    /// The track name.
    /// </summary>
    public string Track => _file.Tag.Title;

    /// <summary>
    /// The album artist name.
    /// </summary>
    public string AlbumArtist => _file.Tag.FirstAlbumArtist;

    /// <summary>
    /// The duration of the track.
    /// </summary>
    public TimeSpan Duration => _file.Properties.Duration;

    /// <summary>
    /// The number of the track.
    /// </summary>
    public int TrackNumber => (int)_file.Tag.Track;

    #endregion Properties

    #region Member

    /// <summary>
    /// The actual file.
    /// </summary>
    private File _file;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="file">Path to the file.</param>
    public LocalFile(string file)
    {
      _file = File.Create(file);
    }
  }
}