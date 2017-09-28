using IF.Lastfm.Core.Objects;
using iTunesLib;
using Last.fm_Scrubbler_WPF.Properties;
using Last.fm_Scrubbler_WPF.ViewModels.ScrobbleViewModels;
using Last.fm_Scrubbler_WPF.Views.ScrobbleViews;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="MediaPlayerScrobbleControl"/>.
  /// </summary>
  class ITunesScrobbleViewModel : MediaPlayerScrobbleViewModelBase, IDisposable
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
    /// Name of the current track.
    /// </summary>
    public override string CurrentTrackName
    {
      get { return ITunesApp?.CurrentTrack?.Name; }
    }

    /// <summary>
    /// Name of the current artist.
    /// </summary>
    public override string CurrentArtistName
    {
      get { return ITunesApp?.CurrentTrack?.Artist; }
    }

    /// <summary>
    /// Name of the current album.
    /// </summary>
    public override string CurrentAlbumName
    {
      get { return ITunesApp?.CurrentTrack?.Album; }
    }

    /// <summary>
    /// Duration of the current track in seconds.
    /// </summary>
    public override int CurrentTrackLength
    {
      get { return (ITunesApp?.CurrentTrack?.Duration).HasValue ? ITunesApp.CurrentTrack.Duration : 0; }
    }

    /// <summary>
    /// When true, tries to connect to iTunes on startup.
    /// </summary>
    public override bool AutoConnect
    {
      get { return Settings.Default.ITunesAutoConnect; }
      set
      {
        Settings.Default.ITunesAutoConnect = value;
        Settings.Default.Save();
        NotifyOfPropertyChange(() => AutoConnect);
      }
    }

    /// <summary>
    /// Gets if the "Disconnect" button should be enabled.
    /// </summary>
    public bool CanDisconnect
    {
      get { return ITunesApp != null; }
    }

    #endregion iTunes

    #endregion Properties

    #region Private Member

    /// <summary>
    /// ID of the current playing track.
    /// Needed to compare the current track and
    /// the track of the refresh timer.
    /// </summary>
    private int _currentTrackID;

    /// <summary>
    /// Cached playcount of the current track.
    /// We use this to check if something is running on repeat.
    /// </summary>
    private int _currentTrackPlayCount;

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
      PercentageToScrobble = 0.5;

      if (AutoConnect)
        Connect();
    }

    /// <summary>
    /// Connects/reconnects to iTunes.
    /// </summary>
    public override async void Connect()
    {
      EnableControls = false;
      CurrentTrackScrobbled = false;

      if (ITunesApp != null)
        Dispose();

      try
      {
        ITunesApp = new iTunesApp();
        //ITunesApp.OnAboutToPromptUserToQuitEvent += _app_AboutToQuitEvent;
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
    public override void Disconnect()
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
    protected override void UpdateCurrentTrackInfo()
    {
      base.UpdateCurrentTrackInfo();
      _currentTrackID = (ITunesApp?.CurrentTrack?.trackID).HasValue ? ITunesApp.CurrentTrack.trackID : 0;
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
        if (ITunesApp?.CurrentTrack?.trackID != _currentTrackID || ITunesApp?.CurrentTrack?.PlayedCount > _currentTrackPlayCount)
        {
          _currentTrackPlayCount = ITunesApp.CurrentTrack.PlayedCount;
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
      Disconnect();
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

          Scrobble s = new Scrobble(CurrentArtistName, CurrentAlbumName, CurrentTrackName, DateTime.Now) { Duration = TimeSpan.FromSeconds(CurrentTrackLength),
                                                                                                           AlbumArtist = (ITunesApp.CurrentTrack as dynamic).AlbumArtist };
          var response = await MainViewModel.CachingScrobbler.ScrobbleAsync(s);
          if (response.Success && response.Status == IF.Lastfm.Core.Api.Enums.LastResponseStatus.Successful)
          {
            OnStatusUpdated(string.Format("Successfully scrobbled {0}!", CurrentTrackName));
            CurrentTrackScrobbled = true;
          }
          else if (response.Status == IF.Lastfm.Core.Api.Enums.LastResponseStatus.Cached)
          {
            OnStatusUpdated(string.Format("Scrobbling of track {0} failed. Scrobble has been cached", CurrentTrackName));
            CurrentTrackScrobbled = true;
          }
          else
            OnStatusUpdated("Error while scrobbling: " + response.Status);
        }
        catch (Exception ex)
        {
          OnStatusUpdated("Fatal error while trying to scrobble currently playing track. Error: " + ex.Message);
        }
        finally
        {
          EnableControls = true;
          _countTimer.Start();
        }
      }
    }
  }
}