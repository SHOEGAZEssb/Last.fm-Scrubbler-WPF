using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using System;

namespace Scrubbler.ViewModels.SubViewModels
{
  /// <summary>
  /// ViewModel for a milestone.
  /// </summary>
  public class MilestoneViewModel : Screen
  {
    #region Properties

    /// <summary>
    /// The track name.
    /// </summary>
    public string TrackName => _track.Name;

    /// <summary>
    /// The artist name.
    /// </summary>
    public string ArtistName => _track.ArtistName;

    /// <summary>
    /// The album name.
    /// </summary>
    public string AlbumName => _track.AlbumName;

    /// <summary>
    /// The album cover.
    /// </summary>
    public Uri Image => _track.Images.Small;

    /// <summary>
    /// The date this milestone was achieved.
    /// </summary>
    public DateTime MilestoneDate => _track.TimePlayed.Value.DateTime;

    /// <summary>
    /// The scrobble number.
    /// </summary>
    public int ScrobbleNumber
    {
      get { return _scrobbleNumber; }
      private set
      {
        _scrobbleNumber = value;
        NotifyOfPropertyChange();
      }
    }
    private int _scrobbleNumber;

    #endregion Properties

    #region Member

    /// <summary>
    /// The scrobble.
    /// </summary>
    private LastTrack _track;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="track">The scrobble.</param>
    /// <param name="scrobbleNumber">The scrobble number.</param>
    public MilestoneViewModel(LastTrack track, int scrobbleNumber)
    {
      _track = track;
      ScrobbleNumber = scrobbleNumber;
    }

    #endregion Construction
  }
}