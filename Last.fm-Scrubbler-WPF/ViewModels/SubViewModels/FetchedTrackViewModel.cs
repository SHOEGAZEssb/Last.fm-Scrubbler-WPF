using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Models;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.TrackResultView"/>.
  /// </summary>
  class FetchedTrackViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Event that triggers when <see cref="ToScrobble"/> changed.
    /// </summary>
    public event EventHandler ToScrobbleChanged;

    /// <summary>
    /// The fetched track.
    /// </summary>
    public Track FetchedTrack
    {
      get { return _fetchedTrack; }
      private set
      {
        _fetchedTrack = value;
        NotifyOfPropertyChange(() => FetchedTrack);
      }
    }
    private Track _fetchedTrack;

    /// <summary>
    /// Gets/sets if this scrobble should be scrobbled.
    /// </summary>
    public bool ToScrobble
    {
      get { return _toScrobble; }
      set
      {
        _toScrobble = value;
        NotifyOfPropertyChange(() => ToScrobble);
        ToScrobbleChanged?.Invoke(this, EventArgs.Empty);
      }
    }
    private bool _toScrobble;

    /// <summary>
    /// The Uri of the small image of the parent album.
    /// </summary>
    public Uri Image
    {
      get { return _image; }
      private set
      {
        _image = value;
        NotifyOfPropertyChange(() => Image);
      }
    }
    private Uri _image;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="fetchedTrack">The fetched track.</param>
    /// <param name="image">The small image of the parent album.</param>
    public FetchedTrackViewModel(Track fetchedTrack, Uri image)
    {
      FetchedTrack = fetchedTrack;
      Image = image;
    }
  }
}