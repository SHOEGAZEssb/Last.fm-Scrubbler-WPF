using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Scrubbler.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scrubbler.Scrobbling.Scrobbler
{
  /// <summary>
  /// ViewModel for the <see cref="ManualScrobbleView"/>.
  /// </summary>
  public class ManualScrobbleViewModel : ScrobbleViewModelBase
  {
    #region Properties

    /// <summary>
    /// Name of the artist to be scrobbled.
    /// </summary>
    public string Artist
    {
      get { return _artist; }
      set
      {
        _artist = value;
        NotifyOfPropertyChange();
        NotifyCanProperties();
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
        NotifyOfPropertyChange();
        NotifyCanProperties();
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
        NotifyOfPropertyChange();
      }
    }
    private string _album;

    /// <summary>
    /// Name of the artist this album is sorted under.
    /// </summary>
    public string AlbumArtist
    {
      get { return _albumArtist; }
      set
      {
        _albumArtist = value;
        NotifyOfPropertyChange();
      }
    }
    private string _albumArtist;

    /// <summary>
    /// Length of the track.
    /// </summary>
    public TimeSpan Duration
    {
      get { return _duration; }
      set
      {
        _duration = value;
        NotifyOfPropertyChange();
      }
    }
    private TimeSpan _duration;

    /// <summary>
    /// Number of times listened to the track.
    /// </summary>
    public int Amount
    {
      get => _amount;
      set
      {
        if(Amount != value)
        {
          _amount = value;
          NotifyOfPropertyChange();
        }
      }
    }
    private int _amount;

    /// <summary>
    /// ViewModel for selecting the time to scrobble.
    /// </summary>
    public ScrobbleTimeViewModel ScrobbleTimeVM
    {
      get { return _scrobbleTimeVM; }
      private set
      {
        _scrobbleTimeVM = value;
        NotifyOfPropertyChange();
      }
    }
    private ScrobbleTimeViewModel _scrobbleTimeVM;

    /// <summary>
    /// Gets if the scrobble button is enabled.
    /// </summary>
    public override bool CanScrobble
    {
      get { return base.CanScrobble && !string.IsNullOrEmpty(Artist) && !string.IsNullOrEmpty(Track); }
    }

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public override bool CanPreview
    {
      get { return !string.IsNullOrEmpty(Artist) && !string.IsNullOrEmpty(Track); }
    }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    public ManualScrobbleViewModel(IExtendedWindowManager windowManager)
      : base(windowManager, "Manual Scrobbler")
    {
      ScrobbleTimeVM = new ScrobbleTimeViewModel();
      Duration = TimeSpan.FromSeconds(1);
      Amount = 1;
    }

    /// <summary>
    /// Scrobbles the track with the given info.
    /// </summary>
    public override async Task Scrobble()
    {
      try
      {
        EnableControls = false;
        OnStatusUpdated($"Trying to scrobble '{Track}'...");

        var scrobbles = CreateScrobbles();
        var response = await Scrobbler.ScrobbleAsync(scrobbles);
        if (response.Success && response.Status == LastResponseStatus.Successful)
          OnStatusUpdated($"Successfully scrobbled '{Track}'");
        else
          OnStatusUpdated($"Error while scrobbling '{Track}': {response.Status}");
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Fatal error while trying to scrobble: {ex.Message}");
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Creates a list with scrobbles that will be scrobbled
    /// with the current configuration.
    /// </summary>
    /// <returns>List with scrobbles.</returns>
    protected override IEnumerable<Scrobble> CreateScrobbles()
    {
      DateTime time = ScrobbleTimeVM.Time;

      var scrobbles = new Scrobble[Amount];
      for (int i = 0; i < Amount; i++)
      {
        scrobbles[i] = new Scrobble(Artist, Album, Track, time) { AlbumArtist = AlbumArtist, Duration = Duration };
        time = time.Subtract(Duration);
      }

      return scrobbles;
    }

    /// <summary>
    /// Notifies the UI that 'Can' properties changed.
    /// </summary>
    private void NotifyCanProperties()
    {
      NotifyOfPropertyChange(nameof(CanScrobble));
      NotifyOfPropertyChange(nameof(CanPreview));
    }
  }
}