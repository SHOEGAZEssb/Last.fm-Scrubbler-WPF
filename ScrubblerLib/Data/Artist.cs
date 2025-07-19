using IF.Lastfm.Core.Objects;
using System;

namespace ScrubblerLib.Data
{
  /// <summary>
  /// Interpret.
  /// </summary>
  public class Artist
  {
    #region Properties

    /// <summary>
    /// Name of this artist.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Mbid of this artist.
    /// </summary>
    public string Mbid { get; private set; }

    /// <summary>
    /// Image of this artist.
    /// </summary>
    public Uri Image { get; private set; }

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">Name of this artist.</param>
    /// <param name="mbid">Mbid of this artist.</param>
    /// <param name="image">Image of this artist.</param>
    public Artist(string name, string mbid, Uri image)
    {
      Name = name;
      Mbid = mbid;
      Image = image;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="artist">Artist to pull info from.</param>
    public Artist(LastArtist artist)
      : this(artist.Name, artist.Mbid, artist.MainImage?.Large)
    { }

    #endregion Construction
  }
}