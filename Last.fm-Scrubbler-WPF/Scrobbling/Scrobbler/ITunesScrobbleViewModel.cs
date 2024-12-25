using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using iTunesLib;
using Scrubbler.Properties;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using IF.Lastfm.Core.Api;
using Scrubbler.Helper;
using DiscordRPC;

namespace Scrubbler.Scrobbling.Scrobbler
{
  /// <summary>
  /// ViewModel for the <see cref="MediaPlayerScrobbleControl"/>.
  /// </summary>
  public class ITunesScrobbleViewModel : MediaPlayerScrobbleViewModelBase, IDisposable
  {
    #region Properties

    /// <summary>
    /// Connection to iTunes.
    /// </summary>
    public iTunesApp ITunesApp
    {
      get { return _iTunesApp; }
      private set
      {
        _iTunesApp = value;
        NotifyOfPropertyChange();
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
        NotifyOfPropertyChange();
      }
    }

    /// <summary>
    /// When true, updates discord rich presence with the
    /// currently playing track and artist.
    /// </summary>
    public override bool UseRichPresence
    {
      get => Settings.Default.UseRichPresenceITunes;
      set
      {
        if (UseRichPresence != value)
        {
          Settings.Default.UseRichPresenceITunes = value;
          NotifyOfPropertyChange();

          if (!UseRichPresence)
            _discordClient.ClearPresence();
        }
      }
    }

    /// <summary>
    /// Gets if the "Disconnect" button should be enabled.
    /// </summary>
    public bool CanDisconnect
    {
      get { return ITunesApp != null; }
    }

    #endregion Properties

    #region Member

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
    /// Lock object to lock the data update.
    /// </summary>
    private readonly object _lockAnchor = new object();

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="trackAPI">Last.fm API object for getting track information.</param>
    /// <param name="albumAPI">Last.fm API object for getting album information.</param>
    /// <param name="artistAPI">Last.fm API object for getting artist information.</param>
    public ITunesScrobbleViewModel(IExtendedWindowManager windowManager, ITrackApi trackAPI, IAlbumApi albumAPI, IArtistApi artistAPI)
      : base(windowManager, "iTunes Scrobbler", trackAPI, albumAPI, artistAPI)
    {
      PercentageToScrobble = 0.5;

      if (AutoConnect)
        Connect();
    }

    /// <summary>
    /// Connects/reconnects to iTunes.
    /// </summary>
    public override void Connect()
    {
      try
      {
        EnableControls = false;

        if (ITunesApp != null)
          Dispose();

        ITunesApp = new iTunesApp();
        ITunesApp.OnPlayerStopEvent += ITunesApp_OnPlayerStopEvent;
        IsConnected = true;

        CountedSeconds = 0;
        CurrentTrackScrobbled = false;
        _countTimer = new Timer(1000);
        _countTimer.Elapsed += CountTimer_Elapsed;
        _countTimer.Start();

        NotifyOfPropertyChange(() => CanDisconnect);

        UpdateCurrentTrackInfo();

        _refreshTimer = new Timer(100);
        _refreshTimer.Elapsed += RefreshTimer_Elapsed;
        _refreshTimer.Start();
      }
      catch (Exception ex)
      {
        OnStatusUpdated("Error connecting to iTunes: " + ex.Message);
        AutoConnect = false;
        IsConnected = false;
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Updates current track info if itunes stops playing.
    /// </summary>
    /// <param name="iTrack">Current itunes track id. Ignored.</param>
    private void ITunesApp_OnPlayerStopEvent(object iTrack)
    {
      UpdateCurrentTrackInfo();
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
      CountedSeconds = 0;
      CurrentTrackScrobbled = false;
      IsConnected = false;
      UpdateCurrentTrackInfo();
      _discordClient.ClearPresence();
      _discordClient.Dispose();
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
    private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      lock (_lockAnchor)
      {
        if (ITunesApp?.CurrentTrack != null && ITunesApp?.CurrentTrack?.trackID != _currentTrackID || ITunesApp?.CurrentTrack?.PlayedCount > _currentTrackPlayCount)
        {
          CurrentTrackScrobbled = false;
          _currentTrackPlayCount = ITunesApp.CurrentTrack.PlayedCount;
          CountedSeconds = 0;

          if (ITunesApp?.PlayerState == ITPlayerState.ITPlayerStatePlaying)
            _countTimer.Start();

          UpdateCurrentTrackInfo();
        }
      }
    }

    /// <summary>
    /// Counts up and scrobbles if the track has been played longer than 50%.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void CountTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      // we check the playerstate manually because the events don't work.
      if (ITunesApp?.PlayerState == ITPlayerState.ITPlayerStatePlaying)
      {
        UpdateNowPlaying().Forget();
        UpdateRichPresence("itunes", "iTunes");

        if (++CountedSeconds == CurrentTrackLengthToScrobble && CurrentTrackScrobbled == false)
        {
          Scrobble().Forget();
          CurrentTrackScrobbled = true;
        }
      }
      else
        _discordClient.ClearPresence();
    }

    /// <summary>
    /// Cleans up.
    /// </summary>
    public void Dispose()
    {
      if (ITunesApp != null)
      {
        // unlink events
        ITunesApp.OnPlayerStopEvent -= ITunesApp_OnPlayerStopEvent;
        _countTimer.Elapsed -= CountTimer_Elapsed;
        _refreshTimer.Elapsed -= RefreshTimer_Elapsed;

        // release resources
        Marshal.ReleaseComObject(ITunesApp);
        ITunesApp = null;
      }

      GC.SuppressFinalize(this);
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
        Scrobble s = null;
        try
        {
          OnStatusUpdated($"Trying to scrobble '{CurrentTrackName}'...");

          // lock while acquiring current data
          lock (_lockAnchor)
          {
            // try to get AlbumArtist
            string albumArtist = string.Empty;
            try
            {
              albumArtist = (ITunesApp.CurrentTrack as dynamic).AlbumArtist;
            }
            catch (RuntimeBinderException)
            {
              // swallow, AlbumArtist doesn't exist for some reason.
            }

            s = new Scrobble(CurrentArtistName, CurrentAlbumName, CurrentTrackName, DateTime.Now)
            {
              Duration = TimeSpan.FromSeconds(CurrentTrackLength),
              AlbumArtist = albumArtist
            };
          }

          var response = await Scrobbler.ScrobbleAsync(s);
          if (response.Success && response.Status == LastResponseStatus.Successful)
          {
            OnStatusUpdated($"Successfully scrobbled '{s.Track}'");
          }
          else
            OnStatusUpdated($"Error while scrobbling '{s.Track}': {response.Status}");
        }
        catch (Exception ex)
        {
          OnStatusUpdated($"Fatal error while trying to scrobble '{s?.Track}': {ex.Message}");
        }
        finally
        {
          EnableControls = true;
        }
      }
    }
  }
}