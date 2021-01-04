using System;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// ViewModel for a <see cref="DatedScrobble"/> that has been
  /// fetched, parsed or gotten from somewhere else.
  /// </summary>
  public class DatedScrobbleViewModel : ScrobbableObjectViewModelBase
  {
    #region Properties

    /// <summary>
    /// The time this scrobble was scrobbled / played.
    /// </summary>
    public DateTime Played => _scrobble.Played;

    /// <summary>
    /// Gets if this track can be scrobbled with the current configuration.
    /// </summary>
    public override bool CanScrobble => base.CanScrobble && _scrobble.Played > DateTime.Now.Subtract(TimeSpan.FromDays(14));

    #endregion Properties

    #region Member

    /// <summary>
    /// The actual scrobble.
    /// </summary>
    private readonly DatedScrobble _scrobble;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="scrobble">The scrobble.</param>
    public DatedScrobbleViewModel(DatedScrobble scrobble)
      : base(scrobble)
    {
      _scrobble = scrobble;
    }
  }
}