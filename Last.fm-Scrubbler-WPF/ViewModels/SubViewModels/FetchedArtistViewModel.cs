using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.ArtistResultView"/>.
  /// </summary>
  class FetchedArtistViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Event that triggers when this artist is clicked on the UI.
    /// </summary>
    public event EventHandler ArtistClicked;

    /// <summary>
    /// The fetched artist.
    /// </summary>
    public LastArtist FetchedArtist
    {
      get { return _fetchedArtist; }
      private set
      {
        _fetchedArtist = value;
        NotifyOfPropertyChange(() => FetchedArtist);
      }
    }
    private LastArtist _fetchedArtist;

    #endregion

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="fetchedArtist">The fetched artist.</param>
    public FetchedArtistViewModel(LastArtist fetchedArtist)
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