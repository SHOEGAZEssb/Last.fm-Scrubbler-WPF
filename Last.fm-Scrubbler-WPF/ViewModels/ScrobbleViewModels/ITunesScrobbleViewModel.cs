using IF.Lastfm.Core.Objects;
using iTunesLib;
using Last.fm_Scrubbler_WPF.Properties;
using System;
using System.Diagnostics;
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
    /// Connection to iTunes.
    /// </summary>
    public iTunesApp ITunesApp
    {
      get { return _iTunesApp; }
      private set
      {
        _iTunesApp = value;
        NotifyOfPropertyChange(() => ITunesApp);
      }
    }
    private iTunesApp _iTunesApp;

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
    /// If the current track is loved on Last.fm.
    /// </summary>
    public bool CurrentTrackLoved
    {
      get { return _currentTrackLoved; }
      private set
      {
        _currentTrackLoved = value;
        NotifyOfPropertyChange(() => CurrentTrackLoved);
      }
    }
    private bool _currentTrackLoved;

    /// <summary>
    /// Name of the current track.
    /// </summary>
    public string CurrentTrackName
    {
      get { return ITunesApp?.CurrentTrack?.Name; }
    }

    /// <summary>
    /// Name of the current artist.
    /// </summary>
    public string CurrentArtistName
    {
      get { return ITunesApp?.CurrentTrack?.Artist; }
    }

    /// <summary>
    /// Name of the current album.
    /// </summary>
    public string CurrentAlbumName
    {
      get { return ITunesApp?.CurrentTrack?.Album; }
    }

    /// <summary>
    /// Duration of the current track in seconds.
    /// </summary>
    public int CurrentTrackLength
    {
      get { return (ITunesApp?.CurrentTrack?.Duration).HasValue ? ITunesApp.CurrentTrack.Duration : 0; }
    }

    /// <summary>
    /// Gets the amount of seconds needed to hear the current
    /// track to scrobble it.
    /// </summary>
    public int CurrentTrackLengthToScrobble
    {
      get { return CurrentTrackLength / 2; }
    }

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
      get { return ITunesApp != null; }
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

    /// <summary>
    /// Gets if the preview button is enabled.
    /// Not needed here.
    /// </summary>
    public override bool CanPreview
    {
      get { throw new NotImplementedException(); }
    }

    #endregion Properties

    #region Private Member

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

    private const string LASTFMURL = "https://www.last.fm/music/";

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

      if (ITunesApp != null)
        Dispose();

      try
      {
        ITunesApp = new iTunesApp();
        ITunesApp.OnAboutToPromptUserToQuitEvent += _app_AboutToQuitEvent;
      }
      catch (Exception ex)
      {
        OnStatusUpdated("Error connecting to iTunes: " + ex.Message);
        AutoConnect = false;
        return;
      }

      CountedSeconds = 0;
      _countTimer = new Timer(1000);
      _countTimer.Elapsed += _countTimer_Elapsed;
      _countTimer.Start();

      NotifyOfPropertyChange(() => CanDisconnect);

      UpdateCurrentTrackInfo();
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
      NotifyOfPropertyChange(() => CanDisconnect);
      CurrentAlbumArtwork = null;
      UpdateCurrentTrackInfo();
    }

    /// <summary>
    /// Gets the info of the currently playing track.
    /// </summary>
    private void UpdateCurrentTrackInfo()
    {
      NotifyOfPropertyChange(() => CurrentTrackName);
      NotifyOfPropertyChange(() => CurrentArtistName);
      NotifyOfPropertyChange(() => CurrentAlbumName);
      NotifyOfPropertyChange(() => CurrentTrackLength);
      NotifyOfPropertyChange(() => CurrentTrackLengthToScrobble);
      NotifyOfPropertyChange(() => ProgressMaximum);
      _currentTrackID = (ITunesApp?.CurrentTrack?.trackID).HasValue ? ITunesApp.CurrentTrack.trackID : 0;
      UpdateLovedInfo();
    }

    /// <summary>
    /// Checks if the current track is loved.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task UpdateLovedInfo()
    {
      var info = await MainViewModel.Client.Track.GetInfoAsync(CurrentTrackName, CurrentArtistName, MainViewModel.Client.Auth.UserSession.Username);
      if (info.Success)
        CurrentTrackLoved = info.Content.IsLoved.Value;
    }

    /// <summary>
    /// Loves / unloves the curren track.
    /// </summary>
    /// <returns>Task.</returns>
    public async Task SwitchLoveState()
    {
      EnableControls = false;

      try
      {
        if (CurrentTrackLoved)
          await MainViewModel.Client.Track.UnloveAsync(CurrentTrackName, CurrentArtistName);
        else
          await MainViewModel.Client.Track.LoveAsync(CurrentTrackName, CurrentArtistName);

        await UpdateLovedInfo();
      }
      catch(Exception ex)
      {
        OnStatusUpdated("Fatal error while loving/unloving track: " + ex.Message);
        EnableControls = true;
      }
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
        if (ITunesApp?.CurrentTrack?.trackID != _currentTrackID)
        {
          CountedSeconds = 0;
          CurrentTrackScrobbled = false;
          UpdateCurrentTrackInfo();
          FetchAlbumArtwork();
        }
      }
    }

    /// <summary>
    /// Counts up and scrobbles if the track has been played longer than 50%.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void _countTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      lock (_lockAnchor)
      {
        if (ITunesApp?.PlayerState == ITPlayerState.ITPlayerStatePlaying)
        {
          CountedSeconds++;
          if (CountedSeconds >= CurrentTrackLengthToScrobble && !CurrentTrackScrobbled)
          {
            // stop count timer to not trigger scrobble multiple times
            _countTimer.Stop();
            Scrobble();
          }
        }
      }
    }

    /// <summary>
    /// Disconnect when iTunes is about to close.
    /// </summary>
    private void _app_AboutToQuitEvent()
    {
      DisconnectFromITunes();
    }

    /// <summary>
    /// Gets the album artwork of the current track.
    /// </summary>
    /// <returns></returns>
    private async Task FetchAlbumArtwork()
    {
      if (CurrentArtistName != null && CurrentAlbumName != null)
      {
        var album = await MainViewModel.Client.Album.GetInfoAsync(CurrentArtistName, CurrentAlbumName);
        CurrentAlbumArtwork = album?.Content.Images.Large;
      }
    }

    /// <summary>
    /// Opens the Last.fm page for the current track.
    /// </summary>
    public void TrackClicked()
    {
      Process.Start(string.Format(LASTFMURL + "{0}/{1}/{2}", CurrentArtistName.Replace(' ', '+'), CurrentAlbumName.Replace(' ', '+'), CurrentTrackName.Replace(' ', '+')));
    }

    /// <summary>
    /// Opens the Last.fm page for the current artist.
    /// </summary>
    public void ArtistClicked()
    {
      Process.Start(string.Format(LASTFMURL + "{0}", CurrentArtistName.Replace(' ', '+')));
    }

    /// <summary>
    /// Opens the Last.fm page for the current track.
    /// </summary>
    public void AlbumClicked()
    {
      Process.Start(string.Format(LASTFMURL + "{0}/{1}", CurrentArtistName.Replace(' ', '+'), CurrentAlbumName.Replace(' ', '+')));
    }

    /// <summary>
    /// Cleans up.
    /// </summary>
    public void Dispose()
    {
      if (ITunesApp != null)
      {
        // unlink events
        ITunesApp.OnAboutToPromptUserToQuitEvent -= _app_AboutToQuitEvent;
        _countTimer.Elapsed -= _countTimer_Elapsed;
        _refreshTimer.Elapsed -= _refreshTimer_Elapsed;

        // release resources
        Marshal.ReleaseComObject(ITunesApp);
        ITunesApp = null;
      }
    }

    /// <summary>
    /// Scrobbles the current song.
    /// </summary>
    /// <returns>Task.</returns>
    public override async Task Scrobble()
    {
      if (CanScrobble)
      {
        EnableControls = false;

        try
        {
          OnStatusUpdated("Trying to scrobble currently playing track...");

          Scrobble s = new Scrobble(CurrentArtistName, CurrentAlbumName, CurrentTrackName, DateTime.Now) { Duration = TimeSpan.FromSeconds(CurrentTrackLength) };
          var response = await MainViewModel.Scrobbler.ScrobbleAsync(s);
          if (response.Success)
          {
            OnStatusUpdated(string.Format("Successfully scrobbled {0}!", CurrentTrackName));
            CurrentTrackScrobbled = true;
          }
          else
            OnStatusUpdated("Error while scrobbling!");
        }
        catch (Exception ex)
        {
          OnStatusUpdated("Fatal error while trying to scrobble currently playing track. Error: " + ex.Message);
        }
        finally
        {
          EnableControls = true;
          // re-enable count timer
          _countTimer.Start();
        }
      }
    }

    /// <summary>
    /// Does nothing here.
    /// </summary>
    public override void Preview()
    {
      throw new NotImplementedException();
    }
  }
}