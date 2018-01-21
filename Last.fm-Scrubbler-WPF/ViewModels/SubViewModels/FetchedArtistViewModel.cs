using Caliburn.Micro;
using Last.fm_Scrubbler_WPF.Models;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.ArtistResultView"/>.
  /// </summary>
  public class FetchedArtistViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Event that triggers when this artist is clicked on the UI.
    /// </summary>
    public event EventHandler ArtistClicked;

    /// <summary>
    /// The fetched artist.
    /// </summary>
    public Artist FetchedArtist
    {
      get { return _fetchedArtist; }
      private set
      {
        _fetchedArtist = value;
        NotifyOfPropertyChange(() => FetchedArtist);
      }
    }
    private Artist _fetchedArtist;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="fetchedArtist">The fetched artist.</param>
    public FetchedArtistViewModel(Artist fetchedArtist)
    {
      FetchedArtist = fetchedArtist;
    }

    /// <summary>
    /// Triggers the <see cref="ArtistClicked"/> event.
    /// </summary>
    public void Clicked()
    {
      ArtistClicked?.Invoke(FetchedArtist, EventArgs.Empty);
    }
  }
}