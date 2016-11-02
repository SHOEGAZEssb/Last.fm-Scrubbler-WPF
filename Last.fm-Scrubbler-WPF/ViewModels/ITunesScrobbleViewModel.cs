using IF.Lastfm.Core.Objects;
using iTunesLib;
using Last.fm_Scrubbler_WPF.Properties;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.ITunesScrobbleView"/>.
  /// </summary>
  class ITunesScrobbleViewModel : ScrobbleViewModelBase, IDisposable
  {
    #region Properties

    #region iTunes

    /// <summary>
    /// Text of the status label.
    /// </summary>
    public string ITunesStatusLabelText
    {
      get { return _app == null ? "Not connected to iTunes" : "Connected to iTunes"; }
    }

    /// <summary>
    /// Test of the connect / reconnect button.
    /// </summary>
    public string ConnectToITunesButtonText
    {
      get { return _app == null ? "Connect to iTunes" : "Reconnect"; }
    }

    /// <summary>
    /// Currently amount of played seconds.
    /// </summary>
    public int CountedSeconds
    {
      get { return _countedSeconds; }
      private set
      {
        _countedSeconds = value;
        NotifyOfPropertyChange(() => CountedSeconds);
      }
    }
    private int _countedSeconds;

    /// <summary>
    /// If the current playing track has been successfully scrobbled.
    /// </summary>
    public bool CurrentTrackScrobbled
    {
      get { return _currentTrackScrobbled; }
      private set
      {
        _currentTrackScrobbled = value;
        NotifyOfPropertyChange(() => CurrentTrackScrobbled);
      }
    }
    private bool _currentTrackScrobbled;

    /// <summary>
    /// The artwork of the current playing album.
    /// </summary>
    public Uri CurrentAlbumArtwork
    {
      get { return _currentAlbumArtwork; }
      private set
      {
        _currentAlbumArtwork = value;
        NotifyOfPropertyChange(() => CurrentAlbumArtwork);
      }
    }
    private Uri _currentAlbumArtwork;

    /// <summary>
    /// Name of the current track.
    /// </summary>
    public string CurrentTrackName
    {
      get { return _currentTrackName; }
      private set
      {
        _currentTrackName = value;
        NotifyOfPropertyChange(() => CurrentTrackName);
      }
    }
    private string _currentTrackName;

    /// <summary>
    /// Name of the current artist.
    /// </summary>
    public string CurrentArtistName
    {
      get { return _currentArtistName; }
      private set
      {
        _currentArtistName = value;
        NotifyOfPropertyChange(() => CurrentArtistName);
      }
    }
    private string _currentArtistName;

    /// <summary>
    /// Name of the current album.
    /// </summary>
    public string CurrentAlbumName
    {
      get { return _currentAlbumName; }
      private set
      {
        _currentAlbumName = value;
        NotifyOfPropertyChange(() => CurrentAlbumName);
      }
    }
    private string _currentAlbumName;

    /// <summary>
    /// Duration of the current track.
    /// </summary>
    public int CurrentTrackLength
    {
      get { return _currentTrackLength; }
      private set
      {
        _currentTrackLength = value;
        NotifyOfPropertyChange(() => CurrentTrackLength);
        NotifyOfPropertyChange(() => ProgressMaximum);
      }
    }
    private int _currentTrackLength;

    public bool AutoConnect
    {
      get { return Settings.Default.AutoConnect; }
      set
      {
        Settings.Default.AutoConnect = value;
        Settings.Default.Save();
        NotifyOfPropertyChange(() => AutoConnect);
      }
    }

    /// <summary>
    /// Maximum of the progress bar.
    /// Basically 50% of the track duration.
    /// </summary>
    public int ProgressMaximum
    {
      get { return CurrentTrackLength / 2; }
    }

    /// <summary>
    /// Gets if the "Disconnect" button should be enabled.
    /// </summary>
    public bool CanDisconnect
    {
      get { return _app != null; }
    }

    #endregion iTunes

    /// <summary>
    /// If certain controls should be enabled.
    /// </summary>
    public override bool EnableControls
    {
      get { return _enableControls; }
      protected set
      {
        _enableControls = value;
        NotifyOfPropertyChange(() => EnableControls);
      }
    }

    /// <summary>
    /// Gets if the client is ready to scrobble.
    /// </summary>
    public override bool CanScrobble
    {
      get { return MainViewModel.Client.Auth.Authenticated; }
    }

    #endregion Properties

    #region Private Member

    /// <summary>
    /// Connection to iTunes.
    /// </summary>
    private iTunesApp _app;

    /// <summary>
    /// ID of the current playing track.
    /// Needed to compare the current track and
    /// the track of the refresh timer.
    /// </summary>
    private int _currentTrackID;

    /// <summary>
    /// Timer to count seconds.
    /// </summary>
    private Timer _countTimer;

    /// <summary>
    /// Timer to refresh current playing song.
    /// This is because the "TrackChanged" event doesn't work.
    /// </summary>
    private Timer _refreshTimer;

    /// <summary>
    /// Lock object to lock the two timer callbacks.
    /// </summary>
    private object _lockAnchor = new object();

    #endregion Private Member

    /// <summary>
    /// Constructor.
    /// </summary>
    public ITunesScrobbleViewModel()
    {
      if (AutoConnect)
        ConnectToITunes();
    }

    /// <summary>
    /// Connects/reconnects to iTunes.
    /// </summary>
    public async void ConnectToITunes()
    {
      EnableControls = false;
      CurrentTrackScrobbled = false;

      if (_app != null)
        Dispose();

      _app = new iTunesApp();
      _app.OnPlayerPlayEvent += _app_OnPlayerPlayEvent;
      _app.OnPlayerStopEvent += _app_OnPlayerStopEvent;

      _countTimer = new Timer(1000);
      _countTimer.Elapsed += _countTimer_Elapsed;
      CountedSeconds = 0;

      if (_app.PlayerState == ITPlayerState.ITPlayerStatePlaying)
        _countTimer.Start();

      NotifyOfPropertyChange(() => ITunesStatusLabelText);
      NotifyOfPropertyChange(() => ConnectToITunesButtonText);
      NotifyOfPropertyChange(() => CanDisconnect);

      GetCurrentTrackInfo();
      await FetchAlbumArtwork();

      _refreshTimer = new Timer(100);
      _refreshTimer.Elapsed += _refreshTimer_Elapsed;
      _refreshTimer.Start();
      EnableControls = true;
    }

    /// <summary>
    /// Disposes the current connection to the iTunes com.
    /// </summary>
    public void DisconnectFromITunes()
    {
      _refreshTimer.Stop();
      _countTimer.Stop();
      Dispose();
      NotifyOfPropertyChange(() => ConnectToITunesButtonText);
      NotifyOfPropertyChange(() => ITunesStatusLabelText);
      NotifyOfPropertyChange(() => CanDisconnect);
      CurrentAlbumArtwork = null;
      GetCurrentTrackInfo();
    }

    /// <summary>
    /// Gets the info of the currently playing track.
    /// </summary>
    private void GetCurrentTrackInfo()
    {
      CurrentTrackName = _app?.CurrentTrack?.Name;
      CurrentArtistName = _app?.CurrentTrack?.Artist;
      CurrentAlbumName = _app?.CurrentTrack?.Album;
      CurrentTrackLength = (_app?.CurrentTrack?.Duration).HasValue ? _app.CurrentTrack.Duration : 0;
      _currentTrackID = (_app?.CurrentTrack?.trackID).HasValue ? _app.CurrentTrack.trackID : 0;
    }

    /// <summary>
    /// Checks if the current playing song is still the same.
    /// If not, prepare for new track.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void _refreshTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      lock (_lockAnchor)
      {
        if (_app?.CurrentTrack?.trackID != _currentTrackID)
        {
          CountedSeconds = 0;
          CurrentTrackScrobbled = false;
          GetCurrentTrackInfo();
          FetchAlbumArtwork();
        }
      }
    }

    /// <summary>
    /// Counts up and scrobbles if the track has been played longer than 50%.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _countTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      lock (_lockAnchor)
      {
        CountedSeconds++;
        if (CountedSeconds >= _app.CurrentTrack.Duration / 2 && !CurrentTrackScrobbled)
        {
          _countTimer.Stop();
          Scrobble();
        }
      }
    }

    /// <summary>
    /// Stops the <see cref="_countTimer"/>
    /// </summary>
    /// <param name="iTrack">Ignored.</param>
    private void _app_OnPlayerStopEvent(object iTrack)
    {
      _countTimer.Stop();
    }

    /// <summary>
    /// Starts the <see cref="_countTimer"/>
    /// </summary>
    /// <param name="iTrack">Ignored.</param>
    private void _app_OnPlayerPlayEvent(object iTrack)
    {
      _countTimer.Start();
    }

    /// <summary>
    /// Gets the album artwork of the current track.
    /// </summary>
    /// <returns></returns>
    private async Task FetchAlbumArtwork()
    {
      var album = await MainViewModel.Client.Album.GetInfoAsync(CurrentArtistName, CurrentAlbumName);
      CurrentAlbumArtwork = album?.Content.Images.Large;
    }

    /// <summary>
    /// Cleans up.
    /// </summary>
    public void Dispose()
    {
      if (_app != null)
      {
        // unlink events
        _app.OnPlayerPlayEvent -= _app_OnPlayerPlayEvent;
        _app.OnPlayerStopEvent -= _app_OnPlayerStopEvent;

        // release resources
        Marshal.ReleaseComObject(_app);
        _app = null;
      }
    }

    /// <summary>
    /// Scrobbles the current song.
    /// </summary>
    /// <returns></returns>
    public override async Task Scrobble()
    {
      if (CanScrobble)
      {
        EnableControls = false;

        OnStatusUpdated("Trying to scrobble currently playing track...");

        Scrobble s = new Scrobble(CurrentArtistName, CurrentAlbumName, CurrentTrackName, DateTime.Now);
        var response = await MainViewModel.Scrobbler.ScrobbleAsync(s);
        if (response.Success)
        {
          OnStatusUpdated("Successfully scrobbled!");
          CurrentTrackScrobbled = true;
        }
        else
          OnStatusUpdated("Error while scrobbling!");

        EnableControls = true;
      }
    }
  }
}