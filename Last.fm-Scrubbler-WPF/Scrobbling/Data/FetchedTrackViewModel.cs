using System;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// ViewModel for the <see cref="TrackResultView"/>.
  /// </summary>
  public class FetchedTrackViewModel : ScrobbableObjectViewModelBase
  {
    #region Properties

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
      : base(fetchedTrack)
    {
      Image = image;
    }
  }
}