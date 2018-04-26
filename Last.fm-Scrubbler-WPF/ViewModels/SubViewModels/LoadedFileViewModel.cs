using Scrubbler.Interfaces;
using System;

namespace Scrubbler.ViewModels.SubViewModels
{
  /// <summary>
  /// ViewModel for a <see cref="ILocalFile"/>.
  /// </summary>
  public class LoadedFileViewModel : ScrobbableObjectViewModelBase
  {
    #region Properties

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