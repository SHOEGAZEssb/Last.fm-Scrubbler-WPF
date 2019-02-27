using Caliburn.Micro;
using System;

namespace Scrubbler.Scrobbling.Data
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
    public string ArtistName
    {
      get => _scrobble.ArtistName;
      set
      {
        if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
          return;

        _scrobble.ArtistName = value;
        NotifyOfPropertyChange();
      }
    }

    /// <summary>
    /// Name of the track.
    /// </summary>
    public string TrackName
    {
      get => _scrobble.TrackName;
      set
      {
        if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
          return;

        _scrobble.TrackName = value;
        NotifyOfPropertyChange();
      }
    }

    /// <summary>
    /// Name of the album.
    /// </summary>
    public string AlbumName
    {
      get => _scrobble.AlbumName;
      set
      {
        _scrobble.AlbumName = value;
        NotifyOfPropertyChange();
      }
    }

    /// <summary>
    /// Name of the album artist.
    /// </summary>
    public string AlbumArtist
    {
      get => _scrobble.AlbumArtist;
      set
      {
        _scrobble.AlbumArtist = value;
        NotifyOfPropertyChange();
      }
    }

    /// <summary>
    /// Length of this track.
    /// </summary>
    public TimeSpan? Duration => _scrobble.Duration;

    #endregion Properties

    #region Member

    /// <summary>
    /// The actual scrobble.
    /// </summary>
    private readonly ScrobbleBase _scrobble;

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