using IF.Lastfm.Core.Objects;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF.ViewModels.ScrobbleViewModels
{
  class SpotifyScrobbleViewModel : MediaPlayerScrobbleViewModelBase
  {
    #region Properties

    public override bool CanScrobble => throw new NotImplementedException();

    public override bool CanPreview => throw new NotImplementedException();

    public override bool EnableControls { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

    public bool IsSpotifyConnected
    {
      get { return _isSpotifyConnected; }
      private set
      {
        _isSpotifyConnected = value;
        NotifyOfPropertyChange(() => IsSpotifyConnected);
      }
    }

    public override string CurrentTrackName => _currentResponse?.Track?.TrackResource?.Name;

    public override string CurrentArtistName => _currentResponse?.Track?.ArtistResource?.Name;

    public override string CurrentAlbumName => _currentResponse?.Track?.AlbumResource?.Name;

    public override int CurrentTrackLength => (int)_currentResponse?.Track?.Length;

    private bool _isSpotifyConnected;

    #endregion Properties

    #region Member

    private SpotifyLocalAPI _spotify;
    private StatusResponse _currentResponse;

    #endregion Member

    public SpotifyScrobbleViewModel()
    {
      _spotify = new SpotifyLocalAPI();
    }

    public override void Connect()
    {
      if (IsSpotifyConnected)
        DisconnectEvents();

      if (!SpotifyLocalAPI.IsSpotifyRunning())
      {
        OnStatusUpdated("Error connecting to Spotify: Client not running");
        return;
      }
      if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning())
      {
        OnStatusUpdated("Error connecting to Spotify: WebHelper not running");
        return;
      }

      if (!_spotify.Connect())
      {
        OnStatusUpdated("Error connecting to Spotify: Unknown error");
        IsSpotifyConnected = false;
      }
      else
      {
        ConnectEvents();
        IsSpotifyConnected = true;
        _currentResponse = _spotify.GetStatus();
        UpdateCurrentTrackInfo();
      }
    }

    private void ConnectEvents()
    {
      _spotify.OnTrackChange += _spotify_OnTrackChange;
      _spotify.OnTrackTimeChange += _spotify_OnTrackTimeChange;
      _spotify.ListenForEvents = true;
    }

    private void _spotify_OnTrackTimeChange(object sender, TrackTimeChangeEventArgs e)
    {
      CountedSeconds = (int)e.TrackTime;

      if (CountedSeconds == CurrentTrackLengthToScrobble)
        Scrobble();
    }

    private void _spotify_OnTrackChange(object sender, TrackChangeEventArgs e)
    {
      if (e.NewTrack.TrackResource.Uri != e.OldTrack.TrackResource.Uri)
        UpdateCurrentTrackInfo();

      CountedSeconds = 0;
      CurrentTrackScrobbled = false;

      // repeat?
    }

    private void DisconnectEvents()
    {
      _spotify.ListenForEvents = false;
      _spotify.OnTrackChange -= _spotify_OnTrackChange;
      _spotify.OnTrackTimeChange -= _spotify_OnTrackTimeChange;
    }

    /// <summary>
    /// Can't happen here.
    /// </summary>
    public override void Preview()
    {
      throw new NotImplementedException();
    }

    public override async Task Scrobble()
    {
      if (CanScrobble && !CurrentTrackScrobbled && !_currentResponse.Track.IsAd())
      {
        EnableControls = false;

        try
        {
          OnStatusUpdated("Trying to scrobble currently playing track...");

          Scrobble s = new Scrobble(CurrentArtistName, CurrentAlbumName, CurrentTrackName, DateTime.Now)
          {
            Duration = TimeSpan.FromSeconds(CurrentTrackLength),
          };

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
        }
      }
    }

    public override void Disconnect()
    {
      throw new NotImplementedException();
    }

    protected override Task UpdateLovedInfo()
    {
      throw new NotImplementedException();
    }

    protected override Task UpdateNowPlaying()
    {
      throw new NotImplementedException();
    }
  }
}