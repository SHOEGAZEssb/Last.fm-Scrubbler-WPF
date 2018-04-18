using Caliburn.Micro;
using Scrubbler.Interfaces;
using System;

namespace Scrubbler.ViewModels
{
  /// <summary>
  /// ViewModel for a <see cref="ILocalFile"/>.
  /// </summary>
  public class LoadedFileViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Event that triggers when <see cref="ToScrobble"/> changed.
    /// </summary>
    public event EventHandler ToScrobbleChanged;

    /// <summary>
    /// The artist name.
    /// </summary>
    public string Artist => _file.Artist;

    /// <summary>
    /// The album name.
    /// </summary>
    public string Album => _file.Album;

    /// <summary>
    /// The track name.
    /// </summary>
    public string Track => _file.Track;

    /// <summary>
    /// The album artist name.
    /// </summary>
    public string AlbumArtist => _file.AlbumArtist;

    /// <summary>
    /// The duration of the track.
    /// </summary>
    public TimeSpan Duration => _file.Duration;

    /// <summary>
    /// The number of the track.
    /// </summary>
    public int TrackNumber => _file.TrackNumber;

    /// <summary>
    /// Indicates if this file should be scrobbled.
    /// </summary>
    public bool ToScrobble
    {
      get { return _toScrobble; }
      set
      {
        _toScrobble = value;
        NotifyOfPropertyChange(() => ToScrobble);
        ToScrobbleChanged?.Invoke(this, EventArgs.Empty);
      }
    }
    private bool _toScrobble;

    #endregion Properties

    #region Member

    /// <summary>
    /// The file.
    /// </summary>
    private ILocalFile _file;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="file">The loaded file.</param>
    public LoadedFileViewModel(ILocalFile file)
    {
      _file = file;
    }
  }
}