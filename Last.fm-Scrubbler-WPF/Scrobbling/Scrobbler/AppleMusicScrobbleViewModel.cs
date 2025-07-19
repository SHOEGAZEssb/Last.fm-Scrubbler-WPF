using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Scrubbler.Helper;
using Scrubbler.Properties;
using Scrubbler.Scrobbling.Data;
using ScrubblerLib.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace Scrubbler.Scrobbling.Scrobbler
{
  /// <summary>
  /// ViewModel for scrobbling Apple Music.
  /// </summary>
  public class AppleMusicScrobbleViewModel : MediaPlayerScrobbleViewModelBase, IDisposable
  {
    #region Properties

    /// <summary>
    /// Name of the current track.
    /// </summary>
    public override string CurrentTrackName => _currentSong?.SongName;

    /// <summary>
    /// Name of the current artist.
    /// </summary>
    public override string CurrentArtistName => _currentSong?.SongArtist;

    /// <summary>
    /// Name of the current album.
    /// </summary>
    public override string CurrentAlbumName => _currentSong?.SongAlbum;

    /// <summary>
    /// Duration of the current track in seconds.
    /// </summary>
    public override int CurrentTrackLength => _currentSong?.SongDuration ?? 0;

    /// <summary>
    /// When true, tries to connect to iTunes on startup.
    /// </summary>
    public override bool AutoConnect
    {
      get { return Settings.Default.AutoConnectAppleMusic; }
      set
      {
        Settings.Default.AutoConnectAppleMusic = value;
        NotifyOfPropertyChange();
      }
    }

    /// <summary>
    /// When true, updates discord rich presence with the
    /// currently playing track and artist.
    /// </summary>
    public override bool UseRichPresence
    {
      get => Settings.Default.UseRichPresenceAppleMusic;
      set
      {
        if (UseRichPresence != value)
        {
          Settings.Default.UseRichPresenceAppleMusic = value;
          NotifyOfPropertyChange();

          if (!UseRichPresence)
            _discordClient.ClearPresence();
        }
      }
    }

    #endregion Properties

    #region Member

    private UIA3Automation _automation;
    private AutomationElement _songPanel;

    private AppleMusicInfo _currentSong;
    private int _currentSongPlayedSeconds = -1;

    /// <summary>
    /// Timer to refresh current playing song.
    /// </summary>
    private Timer _refreshTimer;

    private Timer _countTimer;

    private static readonly Regex ComposerPerformerRegex = new Regex(@"By\s.*?\s\u2014", RegexOptions.Compiled);
    internal static readonly string[] separator = new[] { " \u2014 " };
    private readonly bool _composerAsArtist = false;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="trackAPI">Last.fm API object for getting track information.</param>
    /// <param name="albumAPI">Last.fm API object for getting album information.</param>
    /// <param name="artistAPI">Last.fm API object for getting artist information.</param>
    public AppleMusicScrobbleViewModel(IExtendedWindowManager windowManager, ITrackApi trackAPI, IAlbumApi albumAPI, IArtistApi artistAPI)
      : base(windowManager, "Apple Music Scrobbler", trackAPI, albumAPI, artistAPI)
    {

    }

    #endregion Construction

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Connect()
    {
      try
      {
        EnableControls = false;

        CountedSeconds = 0;
        CurrentTrackScrobbled = false;

        _songPanel = GetSongPanel();
        IsConnected = true;

        _currentSong = null;
        UpdateCurrentTrackInfo();

        _countTimer = new Timer(1000);
        _countTimer.Elapsed += CountTimer_Elapsed;
        _countTimer.Start();

        _refreshTimer = new Timer(1000);
        _refreshTimer.Elapsed += RefreshTimer_Elapsed;
        _refreshTimer.Start();
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Error connecting to Apple Music: {ex.Message}");
        IsConnected = false;
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Disconnect()
    {
      _refreshTimer?.Stop();
      _countTimer?.Stop();
      _automation?.Dispose();
      _automation = null;
      _currentSong = null;
      CountedSeconds = 0;
      CurrentTrackScrobbled = false;
      IsConnected = false;
      _discordClient.ClearPresence();
      UpdateCurrentTrackInfo();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override async Task Scrobble()
    {
      if (CanScrobble)
      {
        EnableControls = false;
        Scrobble s = null;
        try
        {
          OnStatusUpdated($"Trying to scrobble '{CurrentTrackName}'...");

          s = new Scrobble(CurrentArtistName, CurrentAlbumName, CurrentTrackName, DateTime.Now)
          {
            Duration = TimeSpan.FromSeconds(CurrentTrackLength),
            AlbumArtist = _currentSong.SongAlbumArtist
          };

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

    /// <summary>
    /// Disposes this object.
    /// </summary>
    public void Dispose()
    {
      if (_refreshTimer != null)
      {
        _refreshTimer.Stop();
        _refreshTimer.Elapsed -= RefreshTimer_Elapsed;
        _refreshTimer.Dispose();
      }

      if (_countTimer != null)
      {
        _countTimer.Stop();
        _countTimer.Elapsed -= CountTimer_Elapsed;
        _countTimer.Dispose();
      }

      _automation?.Dispose();
      _automation = null;
    }

    private AutomationElement GetSongPanel()
    {
      var amProcesses = Process.GetProcessesByName("AppleMusic");
      if (amProcesses.Length == 0)
        throw new InvalidOperationException("No Apple Music process found");

      // dispose old automation
      _automation?.Dispose();

      _automation = new UIA3Automation();

      var windows = new List<AutomationElement>();
      var w = _automation.GetDesktop()
        .FindFirstDescendant(c => c.ByProcessId(amProcesses[0].Id));

      if (w != null)
        windows.Add(w);

      // if no windows on the normal desktop, search for virtual desktops and add them
      if (windows.Count == 0)
      {
        var vdesktopWin = Application.Attach(amProcesses[0].Id).GetMainWindow(_automation);
        windows.Add(vdesktopWin);
      }

      // get apple music window with info
      AutomationElement songPanel = null;
      var isMiniPlayer = false;
      foreach (var window in windows)
      {
        isMiniPlayer = window.Name == "Mini Player";
        if (isMiniPlayer)
        {
          songPanel = window.FindFirstChild(cf => cf.ByClassName("Microsoft.UI.Content.DesktopChildSiteBridge"));
          if (songPanel != null)
            break;
        }
        else
          songPanel = FindFirstDescendantWithAutomationId(window, "TransportBar");
      }

      if (songPanel == null)
        throw new InvalidOperationException("Apple Music song panel is not initialised or missing");

      return songPanel;
    }

    private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      try
      {
        var songFieldsPanel = _songPanel.FindFirstChild("LCD");
        var songFields = songFieldsPanel?.FindAllChildren(new ConditionFactory(new UIA3PropertyLibrary())
                                         .ByAutomationId("myScrollViewer")) ?? Array.Empty<AutomationElement>();

        if (songFields.Length != 2)
        {
          _currentSong = null;
          return;
        }

        var songNameElement = songFields[0];
        var songAlbumArtistElement = songFields[1];

        // the upper rectangle is the song name; the bottom rectangle is the author/album
        // lower .Bottom = higher up on the screen (?)
        if (songNameElement.BoundingRectangle.Bottom > songAlbumArtistElement.BoundingRectangle.Bottom)
        {
          songNameElement = songFields[1];
          songAlbumArtistElement = songFields[0];
        }

        var songName = songNameElement.Name;
        var songAlbumArtist = songAlbumArtistElement.Name;

        string songArtist = "";
        string songAlbum = "";
        string songPerformer = null;

        // parse song string into album and artist
        try
        {
          var songInfo = ParseSongAlbumArtist(songAlbumArtist, _composerAsArtist);
          songArtist = songInfo.Item1;
          songAlbum = songInfo.Item2;
          songPerformer = songInfo.Item3;
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException($"Error parsing song album artist: {ex.Message}");
        }

        // get duration from slider
        var s = songFieldsPanel.FindFirstChild("LCDScrubber");
        var duration = s.Patterns.RangeValue.Pattern.Maximum;
        var val = s.Patterns.RangeValue.Pattern.Value;

        var newSong = new AppleMusicInfo(songName, songArtist, songAlbum, songArtist)
        {
          SongDuration = (int)duration.Value,
        };

        // only clear out the current song if song is new
        if (_currentSong != newSong)
        {
          _currentSong = newSong;
          CurrentTrackScrobbled = false;
          CountedSeconds = 0;
          _currentSongPlayedSeconds = -1;
          UpdateCurrentTrackInfo();
        }

      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Error while getting Apple Music info: {ex.Message}");
        Disconnect();
      }
    }

    /// <summary>
    /// Counts up and scrobbles if the track has been played longer than 50%.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void CountTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      var songFieldsPanel = _songPanel.FindFirstChild("LCD");
      if (songFieldsPanel == null)
        return; // no song playing?

      var s = songFieldsPanel.FindFirstChild("LCDScrubber");

      // if current > _currentSongPlayedSeconds, then we are not paused
      var current = (int)s.Patterns.RangeValue.Pattern.Value.Value;
      if (_currentSongPlayedSeconds != -1 && current != _currentSongPlayedSeconds)
      {
        UpdateNowPlaying().Forget();
        UpdateRichPresence("applemusic", "Apple Music");

        if (++CountedSeconds == CurrentTrackLengthToScrobble && CurrentTrackScrobbled == false)
        {
          Scrobble().Forget();
          CurrentTrackScrobbled = true;
        }
      }
      else
        _discordClient.ClearPresence();

      _currentSongPlayedSeconds = current;
    }

    // breadth-first search for element with given automation ID.
    // BFS is preferred as the elements we want to find are generally not too deep in the element tree
    private static AutomationElement FindFirstDescendantWithAutomationId(AutomationElement baseElement, string id)
    {
      var nodes = new List<AutomationElement>() { baseElement };
      for (var i = 0; i < nodes.Count; i++)
      {
        var node = nodes[i];
        if (node.Properties.AutomationId.IsSupported && node.AutomationId == id)
          return node;

        nodes.AddRange(node.FindAllChildren());

        // fallback to prevent this taking too long
        if (nodes.Count > 25)
          return null;
      }
      return null;
    }

    private static Tuple<string, string, string> ParseSongAlbumArtist(string songAlbumArtist, bool composerAsArtist)
    {
      string songArtist;
      string songAlbum;
      string songPerformer = null;

      // some classical songs add "By " before the composer's name
      var songComposerPerformer = ComposerPerformerRegex.Matches(songAlbumArtist);
      if (songComposerPerformer.Count > 0)
      {
        var songComposer = songAlbumArtist.Split(separator, StringSplitOptions.None)[0].Remove(0, 3);
        songPerformer = songAlbumArtist.Split(separator, StringSplitOptions.None)[1];
        songArtist = composerAsArtist ? songComposer : songPerformer;
        songAlbum = songAlbumArtist.Split(separator, StringSplitOptions.None)[2];
      }
      else
      {
        // U+2014 is the emdash used by the Apple Music app, not the standard "-" character on the keyboard!
        var songSplit = songAlbumArtist.Split(separator, StringSplitOptions.None);
        if (songSplit.Length > 1)
        {
          songArtist = songSplit[0];
          songAlbum = songSplit[1];
        }
        else
        {
          // no emdash, probably custom music
          // TODO find a better way to handle this?
          songArtist = songSplit[0];
          songAlbum = songSplit[0];
        }
      }

      return new Tuple<string, string, string>(songArtist, songAlbum, songPerformer);
    }
  }
}