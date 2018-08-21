using IF.Lastfm.Core.Objects;
using System;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// A single track of a <see cref="Release"/>.
  /// </summary>
  public class Track
  {
    #region Properties

    /// <summary>
    /// Name of this track.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Name of the artist of this track.
    /// </summary>
    public string ArtistName { get; private set; }

    /// <summary>
    /// Name of the album this track is on.
    /// </summary>
    public string AlbumName { get; private set; }

    /// <summary>
    /// Cover image of this track.
    /// </summary>
    public Uri Image { get; private set; }

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">Name of this track.</param>
    /// <param name="artistName">Name of the artist of this track.</param>
    /// <param name="albumName">Name of the album this track is on.</param>
    /// <param name="image">Cover image of this track.</param>
    public Track(string name, string artistName, string albumName, Uri image)
    {
      Name = name;
      ArtistName = artistName;
      AlbumName = albumName;
      Image = image;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="track">Track to get info from.</param>
    public Track(LastTrack track)
      : this(track.Name, track.ArtistName, track.AlbumName, track.Images?.Large)
    { }

    #endregion Construction
  }
}