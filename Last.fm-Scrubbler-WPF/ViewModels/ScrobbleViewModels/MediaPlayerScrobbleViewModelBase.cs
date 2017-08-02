using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF.ViewModels.ScrobbleViewModels
{
  abstract class MediaPlayerScrobbleViewModelBase : ScrobbleViewModelBase
  {
    #region Properties

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
      private set
      {
        _currentTrackLoved = value;
        NotifyOfPropertyChange(() => CurrentTrackLoved);
      }
    }
    private bool _currentTrackLoved;

    public abstract string CurrentTrackName { get; }
    public abstract string CurrentArtistName { get; }
    public abstract string CurrentAlbumName { get; }
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

    public int CurrentTrackLengthToScrobble
    {
      get { return (int)Math.Ceiling(CurrentTrackLength * PercentageToScrobble); }
    }

    #endregion Properties

    #region Member

    /// <summary>
    /// Base URL to Last.fm music objects.
    /// </summary>
    protected const string LASTFMURL = "https://www.last.fm/music/";

    #endregion Member

    public abstract void Connect();
    public abstract void Disconnect();

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

    protected abstract Task UpdateLovedInfo();
    protected abstract Task UpdateNowPlaying();

    /// <summary>
    /// Gets the album artwork of the current track.
    /// </summary>
    /// <returns>Task.</returns>
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
  }
}