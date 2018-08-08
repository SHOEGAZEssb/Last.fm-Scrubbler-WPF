namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// ViewModel for a <see cref="ILocalFile"/>.
  /// </summary>
  public class LoadedFileViewModel : ScrobbableObjectViewModelBase
  {
    #region Properties

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
      : base(new ScrobbleBase(file.Track, file.Artist, file.Album, file.AlbumArtist, file.Duration))
    {
      _file = file;
    }
  }
}