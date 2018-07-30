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
    /// The time this scrobble was scrobbled / played.
    /// </summary>
    public DateTime Played => _scrobble.Played;

    /// <summary>
    /// Gets if the "Scrobble?" CheckBox is enabled.
    /// </summary>
    public bool IsEnabled
    {
      get { return _scrobble.Played > DateTime.Now.Subtract(TimeSpan.FromDays(14)); }
    }

    #endregion Properties

    #region Member

    /// <summary>
    /// The actual scrobble.
    /// </summary>
    private DatedScrobble _scrobble;

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