using Scrubbler.Scrobbling.Scrobbler;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// ViewModel for a parsed scrobble from a csv file.
  /// </summary>
  public class ParsedCSVScrobbleViewModel : DatedScrobbleViewModel
  {
    #region Properties

    /// <summary>
    /// Gets if the "Scrobble?" CheckBox is enabled.
    /// </summary>
    public override bool CanScrobble => _scrobbleMode == ScrobbleMode.ImportMode || base.CanScrobble;

    #endregion Properties

    #region Member

    /// <summary>
    /// The used scrobble mode.
    /// </summary>
    private readonly ScrobbleMode _scrobbleMode;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parsedScrobble">The scrobble parsed from the csv file.</param>
    /// <param name="scrobbleMode">The current scrobble mode.</param>
    public ParsedCSVScrobbleViewModel(DatedScrobble parsedScrobble, ScrobbleMode scrobbleMode)
      : base(parsedScrobble)
    {
      _scrobbleMode = scrobbleMode;
    }
  }
}