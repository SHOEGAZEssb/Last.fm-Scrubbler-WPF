using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Scrubbler.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;

namespace Scrubbler.Scrobbling.Scrobbler
{
  /// <summary>
  /// Base class for all scrobblers that scrobble a media player.
  /// </summary>
  public abstract class MediaPlayerScrobbleViewModelBase : ScrobbleViewModelBase
  {
    #region Properties

    /// <summary>
    /// Indicates if a connection
    /// to a client is established.
    /// </summary>
    public bool IsConnected
    {
      get { return _isConnected; }
      protected set
      {
        _isConnected = value;
        NotifyOfPropertyChange();
      }
    }
    private bool _isConnected;

    /// <summary>
    /// When true, tries to connect to the media player on startup.
    /// </summary>
    public abstract bool AutoConnect { get; set; }

    /// <summary>
    /// If the current track is loved on Last.fm.
    /// </summary>
    public bool CurrentTrackLoved
    {
      get { return _currentTrackLoved; }
      protected set
      {
        _currentTrackLoved = value;
        NotifyOfPropertyChange();
      }
    }
    private bool _currentTrackLoved;

    /// <summary>
    /// The name of the current playing track.
    /// </summary>
    public abstract string CurrentTrackName { get; }

    /// <summary>
    /// The name of the current artist.
    /// </summary>
    public abstract string CurrentArtistName { get; }

    /// <summary>
    /// The name of the current album.
    /// </summary>
    public abstract string CurrentAlbumName { get; }

    /// <summary>
    /// The length of the current track.
    /// </summary>
    public abstract int CurrentTrackLength { get; }

    /// <summary>
    /// The artwork of the current playing album.
    /// </summary>
    public Uri CurrentAlbumArtwork
    {
      get { return _currentAlbumArtwork; }
      protected set
      {
        _currentAlbumArtwork = value;
        NotifyOfPropertyChange();
      }
    }
    private Uri _currentAlbumArtwork;

    /// <summary>
    /// Amount of times the user has played
    /// the current track.
    /// </summary>
    public int CurrentTrackPlayCount
    {
      get => _currentTrackPlayCount;
      protected set
      {
        _currentTrackPlayCount = value;
        NotifyOfPropertyChange();
      }
    }
    private int _currentTrackPlayCount;

    /// <summary>
    /// Amount of times the user has played
    /// the current artist.
    /// </summary>
    public int CurrentArtistPlayCount
    {
      get => _currentArtistPlayCount;
      protected set
      {
        _currentArtistPlayCount = value;
        NotifyOfPropertyChange();
      }
    }
    private int _currentArtistPlayCount;

    /// <summary>
    /// Amount of times the user has played
    /// the current album.
    /// </summary>
    public int CurrentAlbumPlayCount
    {
      get => _currentAlbumPlayCount;
      protected set
      {
        _currentAlbumPlayCount = value;
        NotifyOfPropertyChange();
      }
    }
    private int _currentAlbumPlayCount;

    /// <summary>
    /// Currently amount of played seconds.
    /// </summary>
    public int CountedSeconds
    {
      get { return _countedSeconds; }
      protected set
      {
        _countedSeconds = value;
        NotifyOfPropertyChange();
      }
    }
    private int _countedSeconds;

    /// <summary>
    /// The factor by which to determine the amount of
    /// seconds to listen to the current song
    /// needed to scrobble the song.
    /// </summary>
    public double PercentageToScrobble
    {
      get { return _percentageToScrobble; }
      set
      {
        double rounded = Math.Round(value, 1);
        if (rounded < 0.5 || rounded > 1.0)
          throw new ArgumentOutOfRangeException(nameof(value), "Percentage to scrobble must be greater or equal 50% and smaller or equal 100%");

        _percentageToScrobble = rounded;
        NotifyOfPropertyChange();
      }
    }
    private double _percentageToScrobble;

    /// <summary>
    /// Seconds needed to listen to the current song to scrobble it.
    /// </summary>
    public int CurrentTrackLengthToScrobble
    {
      get { return (int)Math.Ceiling(CurrentTrackLength * PercentageToScrobble); }
    }

    /// <summary>
    /// Gets if the preview button is enabled.
    /// Not needed here.
    /// </summary>
    public override bool CanPreview => false;

    /// <summary>
    /// Command for switching the love state of a track.
    /// </summary>
    public ICommand SwitchLoveStateCommand { get; }

    #endregion Properties

    #region Member

    /// <summary>
    /// Base URL to Last.fm music objects.
    /// </summary>
    protected const string LASTFMURL = "https://www.last.fm/music/";

    /// <summary>
    /// Base URL to a last.fm user.
    /// </summary>
    protected const string LASTFMUSERURL = "https://www.last.fm/user/";

    /// <summary>
    /// Last.fm API object for getting track information.
    /// </summary>
    protected ITrackApi _trackAPI;

    /// <summary>
    /// Last.fm API object for getting album information.
    /// </summary>
    protected IAlbumApi _albumAPI;

    /// <summary>
    /// Last.fm API object for getting artist information.
    /// </summary>
    protected IArtistApi _artistAPI;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="displayName">Display name.</param>
    /// <param name="trackAPI">Last.fm API object for getting track information.</param>
    /// <param name="albumAPI">Last.fm API object for getting album information.</param>
    /// <param name="artistAPI">Last.fm API object for getting artist information.</param>
    protected MediaPlayerScrobbleViewModelBase(IExtendedWindowManager windowManager, string displayName, ITrackApi trackAPI, IAlbumApi albumAPI, IArtistApi artistAPI)
      : base(windowManager, displayName)
    {
      _trackAPI = trackAPI;
      _albumAPI = albumAPI;
      _artistAPI = artistAPI;
      CurrentTrackPlayCount = -1;
      CurrentAlbumPlayCount = -1;
      CurrentArtistPlayCount = -1;
      SwitchLoveStateCommand = new DelegateCommand((o) => SwitchLoveState().Forget());
    }

    /// <summary>
    /// Connects to the client.
    /// </summary>
    public abstract void Connect();

    /// <summary>
    /// Disconnects from the client.
    /// </summary>
    public abstract void Disconnect();

    /// <summary>
    /// Loves / unloves the current track.
    /// </summary>
    /// <returns>Task.</returns>
    public async Task SwitchLoveState()
    {
      try
      {
        EnableControls = false;

        if (CurrentTrackLoved)
          await _trackAPI.UnloveAsync(CurrentTrackName, CurrentArtistName);
        else
          await _trackAPI.LoveAsync(CurrentTrackName, CurrentArtistName);

        await UpdateLovedInfo();
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Fatal error while loving/unloving '{CurrentTrackName}': {ex.Message}");
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Opens the Last.fm page for the current track.
    /// </summary>
    public void TrackClicked()
    {
      Process.Start($"{LASTFMURL}{GetUrlEncodedString(CurrentArtistName)}/{ GetUrlEncodedString(CurrentAlbumName)}/{GetUrlEncodedString(CurrentTrackName)}");
    }

    /// <summary>
    /// Opens the Last.fm page for the playcounts of
    /// the current track.
    /// </summary>
    public void TrackPlayCountClicked()
    {
      Process.Start($"{LASTFMUSERURL}{Scrobbler.User.Username}/library/music/{GetUrlEncodedString(CurrentArtistName)}/_/{GetUrlEncodedString(CurrentTrackName)}");
    }

    /// <summary>
    /// Opens the Last.fm page for the current artist.
    /// </summary>
    public void ArtistClicked()
    {
      Process.Start($"{LASTFMURL}{GetUrlEncodedString(CurrentArtistName)}");
    }

    /// <summary>
    /// Opens the Last.fm page for the playcounts of
    /// the current artist.
    /// </summary>
    public void ArtistPlayCountClicked()
    {
      Process.Start($"{LASTFMUSERURL}{Scrobbler.User.Username}/library/music/{GetUrlEncodedString(CurrentArtistName)}");
    }

    /// <summary>
    /// Opens the Last.fm page for the current track.
    /// </summary>
    public void AlbumClicked()
    {
      Process.Start($"{LASTFMURL}{GetUrlEncodedString(CurrentArtistName)}/{GetUrlEncodedString(CurrentAlbumName)}");
    }

    /// <summary>
    /// Opens the Last.fm page for the playcounts of
    /// the current album.
    /// </summary>
    public void AlbumPlayCountClicked()
    {
      Process.Start($"{LASTFMUSERURL}{Scrobbler.User.Username}/library/music/{GetUrlEncodedString(CurrentArtistName)}/{GetUrlEncodedString(CurrentAlbumName)}");
    }

    /// <summary>
    /// Notifies the ui of changed song info.
    /// </summary>
    protected virtual void UpdateCurrentTrackInfo()
    {
      NotifyOfPropertyChange(() => CurrentTrackName);
      NotifyOfPropertyChange(() => CurrentArtistName);
      NotifyOfPropertyChange(() => CurrentAlbumName);
      NotifyOfPropertyChange(() => CurrentTrackLength);
      NotifyOfPropertyChange(() => CurrentTrackLengthToScrobble);
      UpdateLovedInfo().Forget();
      UpdateNowPlaying().Forget();
      UpdatePlayCounts().Forget();
      FetchAlbumArtwork().Forget();
    }

    /// <summary>
    /// Does nothing here.
    /// </summary>
    /// <returns>Empty scrobble collection.</returns>
    protected override IEnumerable<Scrobble> CreateScrobbles()
    {
      return Enumerable.Empty<Scrobble>();
    }

    /// <summary>
    /// Checks if the current track is loved.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task UpdateLovedInfo()
    {
      if (CurrentTrackName != null && CurrentArtistName != null && Scrobbler.IsAuthenticated)
      {
        var info = await _trackAPI.GetInfoAsync(CurrentTrackName, CurrentArtistName, Scrobbler.User.Username);
        if (info.Success && info.Status == LastResponseStatus.Successful)
          CurrentTrackLoved = info.Content.IsLoved.Value;
      }
    }

    /// <summary>
    /// Updates the "now playing" info.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task UpdateNowPlaying()
    {
      if (CurrentTrackName != null && CurrentArtistName != null)
        await _trackAPI.UpdateNowPlayingAsync(new Scrobble(CurrentArtistName, CurrentAlbumName, CurrentTrackName, DateTime.Now));
    }

    /// <summary>
    /// Updates the play count of the current playing track.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task UpdatePlayCounts()
    {
      try
      {
        // get track playcount
        var trackResponse = await _trackAPI.GetInfoAsync(CurrentTrackName, CurrentArtistName, Scrobbler.User.Username);
        if (trackResponse.Success && trackResponse.Status == LastResponseStatus.Successful)
          CurrentTrackPlayCount = trackResponse.Content.UserPlayCount ?? -1;
      }
      catch
      {
        CurrentTrackPlayCount = -1;
      }

      try
      {
        // get album playcount
        var albumReponse = await _albumAPI.GetInfoAsync(CurrentArtistName, CurrentAlbumName, false, Scrobbler.User.Username);
        if (albumReponse.Success && albumReponse.Status == LastResponseStatus.Successful)
          CurrentAlbumPlayCount = albumReponse.Content.UserPlayCount ?? -1;
      }
      catch
      {
        CurrentAlbumPlayCount = -1;
      }
    }

    /// <summary>
    /// Gets the album artwork of the current track.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task FetchAlbumArtwork()
    {
      if (CurrentArtistName != null && CurrentAlbumName != null)
      {
        var album = await _albumAPI.GetInfoAsync(CurrentArtistName, CurrentAlbumName);
        CurrentAlbumArtwork = album?.Content?.Images?.Large;
      }
      else
        CurrentAlbumArtwork = null;
    }

    /// <summary>
    /// Replaces special characters with the
    /// corresponding url character.
    /// </summary>
    /// <param name="originalString">String to make url-conform.</param>
    /// <returns>Url-conform string.</returns>
    private static string GetUrlEncodedString(string originalString)
    {
      return HttpUtility.UrlEncode(originalString);
    }
  }
}