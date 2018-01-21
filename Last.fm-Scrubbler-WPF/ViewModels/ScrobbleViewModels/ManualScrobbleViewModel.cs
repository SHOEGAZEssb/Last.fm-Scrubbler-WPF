using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Interfaces;
using Last.fm_Scrubbler_WPF.ViewModels.ScrobbleViewModels;
using Last.fm_Scrubbler_WPF.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="ManualScrobbleView"/>.
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
        NotifyOfPropertyChange(() => Artist);
        NotifyOfPropertyChange(() => CanScrobble);
        NotifyOfPropertyChange(() => CanPreview);
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
        NotifyOfPropertyChange(() => Track);
        NotifyOfPropertyChange(() => CanScrobble);
        NotifyOfPropertyChange(() => CanPreview);
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
        NotifyOfPropertyChange(() => Album);
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
        NotifyOfPropertyChange(() => AlbumArtist);
      }
    }
    private string _albumArtist;

    public TimeSpan Duration
    {
      get { return _duration; }
      set
      {
        _duration = value;
        NotifyOfPropertyChange(() => Duration);
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
        NotifyOfPropertyChange(() => EnableControls);
        NotifyOfPropertyChange(() => CanScrobble);
        NotifyOfPropertyChange(() => CanPreview);
      }
    }

    /// <summary>
    /// Gets if the scrobble button is enabled.
    /// </summary>
    public override bool CanScrobble
    {
      get { return base.CanScrobble && Artist.Length > 0 && Track.Length > 0 && EnableControls; }
    }

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public override bool CanPreview
    {
      get { return Artist.Length > 0 && Track.Length > 0 && EnableControls; }
    }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    public ManualScrobbleViewModel(IWindowManager windowManager, IAuthScrobbler scrobbler)
      : base(windowManager, scrobbler)
    {
      Artist = "";
      Track = "";
      Album = "";
      AlbumArtist = "";
      Duration = TimeSpan.FromSeconds(0);
      UseCurrentTime = true;
    }

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

    protected override IEnumerable<Scrobble> CreateScrobbles()
    {
      return new Scrobble[] { new Scrobble(Artist, Album, Track, Time) { AlbumArtist = AlbumArtist, Duration = Duration } };
    }
  }
}