using IF.Lastfm.Core.Objects;
using System;
using System.Linq;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// Album.
  /// </summary>
  public class Release
  {
    #region Properties

    /// <summary>
    /// Name of this release.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Name of the artist of this release.
    /// </summary>
    public string ArtistName { get; private set; }

    /// <summary>
    /// Mbid of this release.
    /// </summary>
    public string Mbid { get; private set; }

    /// <summary>
    /// Image of this release.
    /// </summary>
    public Uri Image { get; private set; }

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">Name of this release.</param>
    /// <param name="artistName">Name of the artist of this release.</param>
    /// <param name="mbid">Mbid of this release.</param>
    /// <param name="image">Image of this release.</param>
    public Release(string name, string artistName, string mbid, Uri image)
    {
      Name = name;
      ArtistName = artistName;
      Mbid = mbid;
      Image = image;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="release">Release to get data from.</param>
    public Release(LastAlbum release)
      : this(release.Name, release.ArtistName, release.Mbid, release.Images?.Large)
    { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="release">Release to get data from.</param>
    public Release(Hqub.MusicBrainz.API.Entities.ReleaseGroup release)
      : this(release.Title, release.Credits.First().Artist.Name, release.Id, null)
    { }

    #endregion Construction
  }
}