using Scrubbler.Models;
using Scrubbler.ViewModels.ScrobbleViewModels;
using System;

namespace Scrubbler.ViewModels.SubViewModels
{
  /// <summary>
  /// ViewModel for a parsed scrobble from a csv file.
  /// </summary>
  public class ParsedCSVScrobbleViewModel : ScrobbableObjectViewModelBase
  {
    #region Properties

    /// <summary>
    /// The scrobble parsed from the csv file.
    /// </summary>
    public DatedScrobble ParsedScrobble
    {
      get { return _parsedScrobble; }
      private set
      {
        _parsedScrobble = value;
        NotifyOfPropertyChange(() => ParsedScrobble);
        NotifyOfPropertyChange(() => IsEnabled);
      }
    }
    private DatedScrobble _parsedScrobble;

    /// <summary>
    /// Gets if the "Scrobble?" CheckBox is enabled.
    /// </summary>
    public bool IsEnabled
    {
      get { return ParsedScrobble.Played > DateTime.Now.Subtract(TimeSpan.FromDays(14)) || _scrobbleMode == CSVScrobbleMode.ImportMode; }
    }

    #endregion Properties

    #region Private Member

    private CSVScrobbleMode _scrobbleMode;

    #endregion Private Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parsedScrobble">The scrobble parsed from the csv file.</param>
    /// <param name="scrobbleMode">The current scrobble mode.</param>
    public ParsedCSVScrobbleViewModel(DatedScrobble parsedScrobble, CSVScrobbleMode scrobbleMode)
    {
      ParsedScrobble = parsedScrobble;
      _scrobbleMode = scrobbleMode;
    }
  }
}
