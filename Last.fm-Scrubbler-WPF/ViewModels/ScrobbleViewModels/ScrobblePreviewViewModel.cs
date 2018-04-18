using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using System.Collections.Generic;

namespace Scrubbler.ViewModels.ScrobbleViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.ScrobbleViews.ScrobblePreviewView"/>.
  /// </summary>
  public class ScrobblePreviewViewModel : Screen
  {
    #region Properties

    /// <summary>
    /// Scrobbles to preview.
    /// </summary>
    public IEnumerable<Scrobble> Scrobbles
    {
      get { return _scrobbles; }
      private set
      {
        _scrobbles = value;
        NotifyOfPropertyChange();
      }
    }
    private IEnumerable<Scrobble> _scrobbles;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="scrobbles">List with scrobbles to preview.</param>
    public ScrobblePreviewViewModel(IEnumerable<Scrobble> scrobbles)
    {
      Scrobbles = scrobbles;
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