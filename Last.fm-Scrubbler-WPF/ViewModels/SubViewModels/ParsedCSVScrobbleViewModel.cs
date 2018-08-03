using Scrubbler.Models;
using Scrubbler.ViewModels.ScrobbleViewModels;
using System;

namespace Scrubbler.ViewModels.SubViewModels
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
    public new bool IsEnabled
    {
      get { return Played > DateTime.Now.Subtract(TimeSpan.FromDays(14)) || _scrobbleMode == CSVScrobbleMode.ImportMode; }
    }

    #endregion Properties

    #region Member

    /// <summary>
    /// The used scrobble mode.
    /// </summary>
    private readonly CSVScrobbleMode _scrobbleMode;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parsedScrobble">The scrobble parsed from the csv file.</param>
    /// <param name="scrobbleMode">The current scrobble mode.</param>
    public ParsedCSVScrobbleViewModel(DatedScrobble parsedScrobble, CSVScrobbleMode scrobbleMode)
      : base(parsedScrobble)
    {
      _scrobbleMode = scrobbleMode;
    }
  }
}