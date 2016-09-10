using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  class ManualScrobbleViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Event that is triggered when the <see cref="MainViewModel.CurrentStatus"/> should
    /// be updated.
    /// </summary>
    public event EventHandler<UpdateStatusEventArgs> StatusUpdated;

    /// <summary>
    /// Name of the artist to be scrobbled.
    /// </summary>
    public string Artist
    {
      get { return _artist; }
      set
      {
        _artist = value;
        NotifyOfPropertyChange(() => Artist);
        NotifyOfPropertyChange(() => CanScrobble);
      }
    }
    private string _artist;

    /// <summary>
    /// Name of the track to be scrobbled.
    /// </summary>
    public string Track
    {
      get { return _track; }
      set
      {
        _track = value;
        NotifyOfPropertyChange(() => Track);
        NotifyOfPropertyChange(() => CanScrobble);
      }
    }
    private string _track;

    /// <summary>
    /// Name of the album to be scrobbled.
    /// </summary>
    public string Album
    {
      get { return _album; }
      set
      {
        _album = value;
        NotifyOfPropertyChange(() => Album);
      }
    }
    private string _album;

    /// <summary>
    /// Time of the scrobble to scrobbled.
    /// </summary>
    public DateTime TimePlayed
    {
      get { return _timePlayed; }
      set
      {
        _timePlayed = value;
        NotifyOfPropertyChange(() => TimePlayed);
      }
    }
    private DateTime _timePlayed;

    /// <summary>
    /// Gets/sets if the <see cref="TimePlayed"/> is the current time.
    /// </summary>
    public bool CurrentDateTime
    {
      get { return _currentDateTime; }
      set
      {
        _currentDateTime = value;
        if (value)
          TimePlayed = DateTime.Now;

        NotifyOfPropertyChange(() => CurrentDateTime);
      }
    }
    private bool _currentDateTime;

    /// <summary>
    /// Gets if the scrobble button is enabled.
    /// </summary>
    public bool CanScrobble
    {
      get { return MainViewModel.Client.Auth.Authenticated && Artist.Length > 0 && Track.Length > 0; }
    }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    public ManualScrobbleViewModel()
    {
      Artist = "";
      Track = "";
      Album = "";
      MainViewModel.ClientAuthChanged += MainViewModel_ClientAuthChanged;
      CurrentDateTime = true;
      TimePlayed = DateTime.Now;
    }

    /// <summary>
    /// Notifies the <see cref="Views.ManualScrobbleView"/> that the
    /// <see cref="MainViewModel.Client"/> authentication has changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MainViewModel_ClientAuthChanged(object sender, EventArgs e)
    {
      NotifyOfPropertyChange(() => CanScrobble);
    }

    /// <summary>
    /// Scrobbles the track with the given info.
    /// </summary>
    public async void Scrobble()
    {
      StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Trying to scrobble..."));

      if (CurrentDateTime)
        TimePlayed = DateTime.Now;

      Scrobble s = new Scrobble(Artist, Album, Track, TimePlayed);
      var response = await MainViewModel.Scrobbler.ScrobbleAsync(s);
      if (response.Success)
        StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Successfully scrobbled!"));
      else
        StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Error while scrobbling!"));
    }
  }
}