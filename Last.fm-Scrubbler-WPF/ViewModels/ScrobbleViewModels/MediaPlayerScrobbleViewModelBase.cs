using IF.Lastfm.Core.Objects;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF.ViewModels.ScrobbleViewModels
{
  /// <summary>
  /// Base class for all scrobblers that scrobble a media player.
  /// </summary>
  abstract class MediaPlayerScrobbleViewModelBase : ScrobbleViewModelBase
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
        NotifyOfPropertyChange(() => IsConnected);
      }
    }
    private bool _isConnected;

    /// <summary>
    /// If the current playing track has been successfully scrobbled.
    /// </summary>
    public bool CurrentTrackScrobbled
    {
      get { return _currentTrackScrobbled; }
      protected set
      {
        _currentTrackScrobbled = value;
        NotifyOfPropertyChange(() => CurrentTrackScrobbled);
      }
    }
    private bool _currentTrackScrobbled;

    /// <summary>
    /// If the current track is loved on Last.fm.
    /// </summary>
    public bool CurrentTrackLoved
    {
      get { return _currentTrackLoved; }
      protected set
      {
        _currentTrackLoved = value;
        NotifyOfPropertyChange(() => CurrentTrackLoved);
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
        NotifyOfPropertyChange(() => CurrentAlbumArtwork);
      }
    }
    private Uri _currentAlbumArtwork;

    /// <summary>
    /// Currently amount of played seconds.
    /// </summary>
    public int CountedSeconds
    {
      get { return _countedSeconds; }
      protected set
      {
        _countedSeconds = value;
        NotifyOfPropertyChange(() => CountedSeconds);
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

    #region Member

    /// <summary>
    /// Base URL to Last.fm music objects.
    /// </summary>
    protected const string LASTFMURL = "https://www.last.fm/music/";

    #endregion Member

    /// <summary>
    /// Connects to the client.
    /// </summary>
    public abstract void Connect();

    /// <summary>
    /// Disconnects from the client.
    /// </summary>
    public abstract void Disconnect();

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
      UpdateLovedInfo();
      UpdateNowPlaying();
    }

    /// <summary>
    /// Checks if the current track is loved.
    /// </summary>
    /// <returns>Task.</returns>
    protected async Task UpdateLovedInfo()
    {
      if (CurrentTrackName != null && CurrentArtistName != null && MainViewModel.Client.Auth.UserSession != null)
      {
        var info = await MainViewModel.Client.Track.GetInfoAsync(CurrentTrackName, CurrentArtistName, MainViewModel.Client.Auth.UserSession.Username);
        if (info.Success)
          CurrentTrackLoved = info.Content.IsLoved.Value;
      }
    }

    /// <summary>
    /// Updates the "now playing" info.
    /// </summary>
    /// <returns></returns>
    protected async Task UpdateNowPlaying()
    {
      if (CurrentTrackName != null && CurrentArtistName != null)
        await MainViewModel.Client.Track.UpdateNowPlayingAsync(new Scrobble(CurrentArtistName, CurrentAlbumName, CurrentTrackName, DateTime.Now));
    }

    /// <summary>
    /// Loves / unloves the current track.
    /// </summary>
    /// <returns></returns>
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
      catch (Exception ex)
      {
        OnStatusUpdated("Fatal error while loving/unloving track: " + ex.Message);
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Gets the album artwork of the current track.
    /// </summary>
    /// <returns>Task.</returns>
    protected async Task FetchAlbumArtwork()
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
    /// Does nothing here.
    /// </summary>
    public override void Preview()
    {
      throw new NotImplementedException();
    }
  }
}