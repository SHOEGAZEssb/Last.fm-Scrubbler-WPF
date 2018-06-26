using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using iTunesLib;
using Scrubbler.Interfaces;
using Scrubbler.Properties;
using Scrubbler.Views.ScrobbleViews;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using IF.Lastfm.Core.Api;

namespace Scrubbler.ViewModels.ScrobbleViewModels
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
    /// Gets if the "Disconnect" button should be enabled.
    /// </summary>
    public bool CanDisconnect
    {
      get { return ITunesApp != null; }
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

    #endregion Private Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="trackAPI">Last.fm API object for getting track information.</param>
    /// <param name="albumAPI">Last.fm API object for getting album information.</param>
    /// <param name="lastAuth">Last.fm authentication object.</param>
    public ITunesScrobbleViewModel(IExtendedWindowManager windowManager, ITrackApi trackAPI, IAlbumApi albumAPI, ILastAuth lastAuth)
      : base(windowManager, "ITunes Scrobbler", trackAPI, albumAPI, lastAuth)
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
        //ITunesApp.OnAboutToPromptUserToQuitEvent += _app_AboutToQuitEvent;
        IsConnected = true;

        CountedSeconds = 0;
        _countTimer = new Timer(1000);
        _countTimer.Elapsed += _countTimer_Elapsed;
        _countTimer.Start();

        NotifyOfPropertyChange(() => CanDisconnect);

        UpdateCurrentTrackInfo();

        _refreshTimer = new Timer(100);
        _refreshTimer.Elapsed += _refreshTimer_Elapsed;
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
      IsConnected = false;
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
        if (ITunesApp?.CurrentTrack != null && ITunesApp?.CurrentTrack?.trackID != _currentTrackID || ITunesApp?.CurrentTrack?.PlayedCount > _currentTrackPlayCount)
        {
          _currentTrackPlayCount = ITunesApp.CurrentTrack.PlayedCount;
          CountedSeconds = 0;

          if(ITunesApp?.PlayerState == ITPlayerState.ITPlayerStatePlaying)
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
    private void _countTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      // we check the playerstate manually because the events don't work.
      if (ITunesApp?.PlayerState == ITPlayerState.ITPlayerStatePlaying)
      {
        if (++CountedSeconds == CurrentTrackLengthToScrobble)
        {
          // stop count timer to not trigger scrobble multiple times
          _countTimer.Stop();
          Scrobble().Forget(); ;
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
      if (EnableControls && CanScrobble)
      {
        EnableControls = false;
        Scrobble s = null;
        try
        {
          OnStatusUpdated(string.Format("Trying to scrobble '{0}'...", CurrentTrackName));

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

          var response = await Scrobbler.ScrobbleAsync(s, true);
          if (response.Success && response.Status == LastResponseStatus.Successful)
            OnStatusUpdated(string.Format("Successfully scrobbled '{0}'", s.Track));
          else if (response.Status == LastResponseStatus.Cached)
            OnStatusUpdated(string.Format("Scrobbling '{0}' failed. Scrobble has been cached", s.Track));
          else
            OnStatusUpdated(string.Format("Error while scrobbling '{0}': {1}", s.Track, response.Status));
        }
        catch (Exception ex)
        {
          OnStatusUpdated(string.Format("Fatal error while trying to scrobble '{0}': {1}", s?.Track, ex.Message));
        }
        finally
        {
          EnableControls = true;
        }
      }
    }
  }
}