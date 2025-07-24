using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using Scrubbler.Scrobbling.Data;
using ScrubblerLib.Data;
using System.Collections.Generic;
using System.Linq;

namespace Scrubbler.Scrobbling
{
  /// <summary>
  /// ViewModel for the <see cref="ScrobblePreviewView"/>.
  /// </summary>
  public class ScrobblePreviewViewModel : Screen
  {
    #region Properties

    /// <summary>
    /// Scrobbles to preview.
    /// </summary>
    public IEnumerable<DatedScrobbleViewModel> Scrobbles
    {
      get { return _scrobbles; }
      private set
      {
        _scrobbles = value;
        NotifyOfPropertyChange();
      }
    }
    private IEnumerable<DatedScrobbleViewModel> _scrobbles;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="scrobbles">List with scrobbles to preview.</param>
    public ScrobblePreviewViewModel(IEnumerable<Scrobble> scrobbles)
    {
      Scrobbles = scrobbles.Select(s => new DatedScrobbleViewModel(new DatedScrobble(s)));
    }

    /// <summary>
    /// Closes the screen.
    /// </summary>
    public void OK()
    {
      TryClose(true);
    }
  }
}