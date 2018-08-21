using Caliburn.Micro;
using System;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// ViewModel for the <see cref="ArtistResultView"/>.
  /// </summary>
  public class FetchedArtistViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Event that triggers when this artist is clicked on the UI.
    /// </summary>
    public event EventHandler ArtistClicked;

    /// <summary>
    /// Name of this artist.
    /// </summary>
    public string Name => _fetchedArtist.Name;

    /// <summary>
    /// Mbid of this artist.
    /// </summary>
    public Uri Image => _fetchedArtist.Image;

    #endregion Properties

    #region Member

    /// <summary>
    /// The fetched artist.
    /// </summary>
    private Artist _fetchedArtist;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="fetchedArtist">The fetched artist.</param>
    public FetchedArtistViewModel(Artist fetchedArtist)
    {
      _fetchedArtist = fetchedArtist;
    }

    #endregion Construction

    /// <summary>
    /// Triggers the <see cref="ArtistClicked"/> event.
    /// </summary>
    public void Clicked()
    {
      ArtistClicked?.Invoke(_fetchedArtist, EventArgs.Empty);
    }
  }
}