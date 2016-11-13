using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Views;
using System.Collections.Generic;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="ScrobblePreviewView"/>.
  /// </summary>
  public class ScrobblePreviewViewModel : PropertyChangedBase
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
        NotifyOfPropertyChange(() => Scrobbles);
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
    /// Closes the <paramref name="view"/>.
    /// </summary>
    /// <param name="view">View to close.</param>
    public void OK(ScrobblePreviewView view)
    {
      view.Close();
    }
  }
}