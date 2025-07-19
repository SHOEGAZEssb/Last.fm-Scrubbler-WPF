using ScrubblerLib.Data;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// ViewModel for a <see cref="MediaDBScrobble"/>.
  /// </summary>
  public class MediaDBScrobbleViewModel : DatedScrobbleViewModel
  {
    #region Properties

    /// <summary>
    /// The play count of this database entry.
    /// </summary>
    public int PlayCount => _scrobble.PlayCount;

    #endregion Properties

    #region Member

    /// <summary>
    /// The actual scrobble.
    /// </summary>
    private readonly MediaDBScrobble _scrobble;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="scrobble">The parsed scrobble.</param>
    public MediaDBScrobbleViewModel(MediaDBScrobble scrobble)
      : base(scrobble)
    {
      _scrobble = scrobble;
    }
  }
}