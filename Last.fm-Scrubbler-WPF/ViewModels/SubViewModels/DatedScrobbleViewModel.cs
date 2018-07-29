using Scrubbler.Models;
using System;

namespace Scrubbler.ViewModels.SubViewModels
{
  /// <summary>
  /// ViewModel for a <see cref="DatedScrobble"/> that has been
  /// fetched, parsed or gotten from somewhere else.
  /// </summary>
  public class DatedScrobbleViewModel : ScrobbableObjectViewModelBase
  {
    #region Properties

    /// <summary>
    /// The scrobble.
    /// </summary>
    public DatedScrobble Scrobble
    {
      get { return _parsedScrobble; }
      private set
      {
        _parsedScrobble = value;
        NotifyOfPropertyChange(() => Scrobble);
        NotifyOfPropertyChange(() => IsEnabled);
      }
    }
    private DatedScrobble _parsedScrobble;

    /// <summary>
    /// Gets if the "Scrobble?" CheckBox is enabled.
    /// </summary>
    public bool IsEnabled
    {
      get { return Scrobble.Played > DateTime.Now.Subtract(TimeSpan.FromDays(14)); }
    }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="scrobble">The scrobble.</param>
    public DatedScrobbleViewModel(DatedScrobble scrobble)
    {
      Scrobble = scrobble;
    }
  }
}