using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using Scrubbler.Models;
using System.Collections.Generic;
using System.Linq;

namespace Scrubbler.ViewModels.SubViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.SubViews.ScrobblePreviewView"/>.
  /// </summary>
  public class ScrobblePreviewViewModel : Screen
  {
    #region Properties

    /// <summary>
    /// Scrobbles to preview.
    /// </summary>
    public IEnumerable<ScrobbleViewModel> Scrobbles
    {
      get { return _scrobbles; }
      private set
      {
        _scrobbles = value;
        NotifyOfPropertyChange();
      }
    }
    private IEnumerable<ScrobbleViewModel> _scrobbles;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="scrobbles">List with scrobbles to preview.</param>
    public ScrobblePreviewViewModel(IEnumerable<Scrobble> scrobbles)
    {
      Scrobbles = scrobbles.Select(s => new ScrobbleViewModel(new ScrobbleBase(s)));
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