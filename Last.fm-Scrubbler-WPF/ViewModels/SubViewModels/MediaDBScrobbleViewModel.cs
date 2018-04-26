using Scrubbler.Models;

namespace Scrubbler.ViewModels.SubViewModels
{
  /// <summary>
  /// ViewModel for a <see cref="MediaDBScrobble"/>.
  /// </summary>
  public class MediaDBScrobbleViewModel : ScrobbableObjectViewModelBase
  {
    #region Properties

    /// <summary>
    /// The parsed scrobble.
    /// </summary>
    public MediaDBScrobble Scrobble
    {
      get { return _scrobble; }
      private set
      {
        _scrobble = value;
        NotifyOfPropertyChange(() => Scrobble);
      }
    }
    private MediaDBScrobble _scrobble;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="scrobble">The parsed scrobble.</param>
    public MediaDBScrobbleViewModel(MediaDBScrobble scrobble)
    {
      Scrobble = scrobble;
    }
  }
}