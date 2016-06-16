using Caliburn.Micro;
using Last.fm_Scrubbler_WPF.Models;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  class ParsedCSVScrobbleViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Event that triggers when <see cref="ToScrobble"/> changed.
    /// </summary>
    public event EventHandler ToScrobbleChanged;

    /// <summary>
    /// The scrobble parsed from the csv file.
    /// </summary>
    public CSVScrobble ParsedScrobble
    {
      get { return _parsedScrobble; }
      private set
      {
        _parsedScrobble = value;
        NotifyOfPropertyChange(() => ParsedScrobble);
        NotifyOfPropertyChange(() => IsEnabled);
      }
    }
    private CSVScrobble _parsedScrobble;

    /// <summary>
    /// Indicates if this scrobble should be scrobbled.
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

    /// <summary>
    /// Gets if the "Scrobble?" CheckBox is enabled.
    /// </summary>
    public bool IsEnabled
    {
      get { return ParsedScrobble.DateTime > DateTime.Now.Subtract(TimeSpan.FromDays(14)) || _scrobbleMode == CSVScrobbleMode.ImportMode; }
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
    public ParsedCSVScrobbleViewModel(CSVScrobble parsedScrobble, CSVScrobbleMode scrobbleMode)
    {
      ParsedScrobble = parsedScrobble;
      _scrobbleMode = scrobbleMode;
    }
  }
}
