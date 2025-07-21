using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Scrubbler.Helper;
using ScrubblerLib.Scrobbler;
using System;
using System.Collections.Generic;
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
      get { return _scrobbleFeature.Artist; }
      set
      {
        _scrobbleFeature.Artist = value;
        NotifyOfPropertyChange();
        NotifyCanProperties();
      }
    }

    /// <summary>
    /// Name of the track to be scrobbled.
    /// </summary>
    public string Track
    {
      get { return _scrobbleFeature.Track; }
      set
      {
        _scrobbleFeature.Track = value;
        NotifyOfPropertyChange();
        NotifyCanProperties();
      }
    }

    /// <summary>
    /// Name of the album to be scrobbled.
    /// </summary>
    public string Album
    {
      get { return _scrobbleFeature.Album; }
      set
      {
        _scrobbleFeature.Album = value;
        NotifyOfPropertyChange();
      }
    }

    /// <summary>
    /// Name of the artist this album is sorted under.
    /// </summary>
    public string AlbumArtist
    {
      get { return _scrobbleFeature.AlbumArtist; }
      set
      {
        _scrobbleFeature.AlbumArtist = value;
        NotifyOfPropertyChange();
      }
    }

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
      get => _scrobbleFeature.Amount;
      set
      {
        if(Amount != value)
        {
          _scrobbleFeature.Amount = value;
          NotifyOfPropertyChange();
        }
      }
    }

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

    private readonly ManualScrobbleFeature _scrobbleFeature = new ManualScrobbleFeature();

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
      _scrobbleFeature.Timestamp = ScrobbleTimeVM.Time;
      return _scrobbleFeature.CreateScrobbles();
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