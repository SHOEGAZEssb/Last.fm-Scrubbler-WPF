using Scrubbler.Models;
using System;

namespace Scrubbler.ViewModels.SubViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.TrackResultView"/>.
  /// </summary>
  public class FetchedTrackViewModel : ScrobbableObjectViewModelBase
  {
    #region Properties

    /// <summary>
    /// The fetched track.
    /// </summary>
    public ScrobbleBase FetchedTrack
    {
      get { return _fetchedTrack; }
      private set
      {
        _fetchedTrack = value;
        NotifyOfPropertyChange(() => FetchedTrack);
      }
    }
    private ScrobbleBase _fetchedTrack;

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
    public FetchedTrackViewModel(ScrobbleBase fetchedTrack, Uri image)
    {
      FetchedTrack = fetchedTrack;
      Image = image;
    }
  }
}