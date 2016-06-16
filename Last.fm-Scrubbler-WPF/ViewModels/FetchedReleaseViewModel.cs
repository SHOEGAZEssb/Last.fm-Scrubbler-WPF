using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.ReleaseResultView"/>.
  /// </summary>
  class FetchedReleaseViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Event that triggers when this release is clicked on the UI.
    /// </summary>
    public event EventHandler ReleaseClicked;

    /// <summary>
    /// The fetched release.
    /// </summary>
    public LastAlbum FetchedRelease
    {
      get { return _fetchedRelease; }
      private set
      {
        _fetchedRelease = value;
        NotifyOfPropertyChange(() => FetchedRelease);
      }
    }
    private LastAlbum _fetchedRelease;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="fetchedRelease">The fetched release.</param>
    public FetchedReleaseViewModel(LastAlbum fetchedRelease)
    {
      FetchedRelease = fetchedRelease;
    }

    /// <summary>
    /// Triggers the <see cref="ReleaseClicked"/> event.
    /// </summary>
    public void Clicked()
    {
      ReleaseClicked?.Invoke(FetchedRelease, EventArgs.Empty);
    }
  }
}