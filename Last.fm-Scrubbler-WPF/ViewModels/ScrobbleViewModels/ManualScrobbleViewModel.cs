using IF.Lastfm.Core.Objects;
using Scrubbler.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scrubbler.ViewModels.ScrobbleViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.ScrobbleViews.ManualScrobbleView"/>.
  /// </summary>
  public class ManualScrobbleViewModel : ScrobbleTimeViewModelBase
  {
    #region Properties

    /// <summary>
    /// Name of the artist to be scrobbled.
    /// </summary>
    public string Artist
    {
      get { return _artist; }
      set
      {
        _artist = value;
        NotifyOfPropertyChange();
        NotifyCanProperties();
      }
    }
    private string _artist;

    /// <summary>
    /// Name of the track to be scrobbled.
    /// </summary>
    public string Track
    {
      get { return _track; }
      set
      {
        _track = value;
        NotifyOfPropertyChange();
        NotifyCanProperties();
      }
    }
    private string _track;

    /// <summary>
    /// Name of the album to be scrobbled.
    /// </summary>
    public string Album
    {
      get { return _album; }
      set
      {
        _album = value;
        NotifyOfPropertyChange();
      }
    }
    private string _album;

    /// <summary>
    /// Name of the artist this album is sorted under.
    /// </summary>
    public string AlbumArtist
    {
      get { return _albumArtist; }
      set
      {
        _albumArtist = value;
        NotifyOfPropertyChange();
      }
    }
    private string _albumArtist;

    /// <summary>
    /// Length of the track.
    /// </summary>
    public TimeSpan Duration
    {
      get { return _duration; }
      set
      {
        _duration = value;
        NotifyOfPropertyChange();
      }
    }
    private TimeSpan _duration;

    /// <summary>
    /// Gets if certain controls that modify the
    /// scrobbling data are enabled.
    /// </summary>
    public override bool EnableControls
    {
      get { return _enableControls; }
      protected set
      {
        _enableControls = value;
        NotifyOfPropertyChange();
        NotifyCanProperties();
      }
    }

    /// <summary>
    /// Gets if the scrobble button is enabled.
    /// </summary>
    public override bool CanScrobble
    {
      get { return base.CanScrobble && !string.IsNullOrEmpty(Artist) && !string.IsNullOrEmpty(Track) && EnableControls; }
    }

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public override bool CanPreview
    {
      get { return !string.IsNullOrEmpty(Artist) && !string.IsNullOrEmpty(Track) && EnableControls; }
    }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    public ManualScrobbleViewModel(IExtendedWindowManager windowManager)
      : base(windowManager, "Manual Scrobbler")
    { }

    /// <summary>
    /// Scrobbles the track with the given info.
    /// </summary>
    public override async Task Scrobble()
    {
      EnableControls = false;

      try
      {
        OnStatusUpdated("Trying to scrobble...");

        Scrobble s = new Scrobble(Artist, Album, Track, Time) { AlbumArtist = AlbumArtist, Duration = Duration };
        var response = await Scrobbler.ScrobbleAsync(s);
        if (response.Success)
          OnStatusUpdated("Successfully scrobbled!");
        else
          OnStatusUpdated("Error while scrobbling!");
      }
      catch (Exception ex)
      {
        OnStatusUpdated("Fatal error while trying to scrobble: " + ex.Message);
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Creates a list with scrobbles that will be scrobbled
    /// with the current configuration.
    /// </summary>
    /// <returns>List with scrobbles.</returns>
    protected override IEnumerable<Scrobble> CreateScrobbles()
    {
      return new[] { new Scrobble(Artist, Album, Track, Time) { AlbumArtist = AlbumArtist, Duration = Duration } };
    }

    /// <summary>
    /// Notifies the UI that 'Can' properties changed.
    /// </summary>
    private void NotifyCanProperties()
    {
      NotifyOfPropertyChange(() => CanScrobble);
      NotifyOfPropertyChange(() => CanPreview);
    }
  }
}