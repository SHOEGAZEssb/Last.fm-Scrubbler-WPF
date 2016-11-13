using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Views;
using System;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="ManualScrobbleView"/>.
  /// </summary>
  class ManualScrobbleViewModel : ScrobbleViewModelBase
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
    /// Time of the scrobble to scrobbled.
    /// </summary>
    public DateTime TimePlayed
    {
      get { return _timePlayed; }
      set
      {
        _timePlayed = value;
        NotifyOfPropertyChange(() => TimePlayed);
      }
    }
    private DateTime _timePlayed;

    /// <summary>
    /// Gets/sets if the <see cref="TimePlayed"/> is the current time.
    /// </summary>
    public bool CurrentDateTime
    {
      get { return _currentDateTime; }
      set
      {
        _currentDateTime = value;
        if (value)
          TimePlayed = DateTime.Now;

        NotifyOfPropertyChange(() => CurrentDateTime);
      }
    }
    private bool _currentDateTime;

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
      get { return MainViewModel.Client.Auth.Authenticated && Artist.Length > 0 && Track.Length > 0 && EnableControls; }
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
    public ManualScrobbleViewModel()
    {
      Artist = "";
      Track = "";
      Album = "";
      CurrentDateTime = true;
      TimePlayed = DateTime.Now;
    }

    /// <summary>
    /// Scrobbles the track with the given info.
    /// </summary>
    public override async Task Scrobble()
    {
      EnableControls = false;

      OnStatusUpdated("Trying to scrobble...");

      if (CurrentDateTime)
        TimePlayed = DateTime.Now;

      Scrobble s = new Scrobble(Artist, Album, Track, TimePlayed);
      var response = await MainViewModel.Scrobbler.ScrobbleAsync(s);
      if (response.Success)
        OnStatusUpdated("Successfully scrobbled!");
      else
        OnStatusUpdated("Error while scrobbling!");

      EnableControls = true;
    }

    /// <summary>
    /// Preview the track that will be scrobbled.
    /// </summary>
    public override void Preview()
    {
      ScrobblePreviewView spv = new ScrobblePreviewView(new ScrobblePreviewViewModel(new Scrobble[] { new Scrobble(Artist, Album, Track, TimePlayed) }));
      spv.ShowDialog();
    }
  }
}