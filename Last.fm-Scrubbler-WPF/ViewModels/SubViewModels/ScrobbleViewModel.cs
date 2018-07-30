using Caliburn.Micro;
using Scrubbler.Models;
using System;

namespace Scrubbler.ViewModels.SubViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="ScrobbleBase"/>.
  /// </summary>
  public class ScrobbleViewModel : Screen
  {
    #region Properties

    /// <summary>
    /// Name of the artist.
    /// </summary>
    public string ArtistName => _scrobble.ArtistName;

    /// <summary>
    /// Name of the track.
    /// </summary>
    public string TrackName => _scrobble.TrackName;

    /// <summary>
    /// Name of the album.
    /// </summary>
    public string AlbumName => _scrobble.AlbumName;

    /// <summary>
    /// Name of the album artist.
    /// </summary>
    public string AlbumArtist => _scrobble.AlbumArtist;

    /// <summary>
    /// Length of this track.
    /// </summary>
    public TimeSpan? Duration => _scrobble.Duration;

    #endregion Properties

    #region Member

    /// <summary>
    /// The actual scrobble.
    /// </summary>
    private ScrobbleBase _scrobble;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="scrobble">The actual scrobble.</param>
    public ScrobbleViewModel(ScrobbleBase scrobble)
    {
      _scrobble = scrobble;
    }

    #endregion Construction
  }
}