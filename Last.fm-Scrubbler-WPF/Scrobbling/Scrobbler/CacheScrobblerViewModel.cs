using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Scrubbler.Helper;
using Scrubbler.Properties;
using Scrubbler.Scrobbling.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Scrubbler.Scrobbling.Scrobbler
{
  /// <summary>
  /// ViewModel for the <see cref="CacheScrobblerView"/>
  /// </summary>
  public class CacheScrobblerViewModel : ScrobbleMultipleViewModelBase<DatedScrobbleViewModel>
  {
    #region Properties

    /// <summary>
    /// Gets if the scrobble button is enabled.
    /// </summary>
    public override bool CanScrobble => base.CanScrobble && Scrobbles.Count > 0 && EnableControls;

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public override bool CanPreview => Scrobbles.Count > 0 && EnableControls;

    /// <summary>
    /// Gets if certain controls that modify the
    /// scrobbling data are enabled.
    /// </summary>
    public override bool EnableControls
    {
      get { return _enableControls; }
      protected set
      {
        _enableControls = value;
        NotifyOfPropertyChange();
        NotifyOfPropertyChange(() => CanScrobble);
        NotifyOfPropertyChange(() => CanPreview);
      }
    }

    /// <summary>
    /// If true, tries to scrobble the cache at application startup.
    /// </summary>
    public bool AutoScrobble
    {
      get { return Settings.Default.AutoScrobbleCache; }
      set
      {
        Settings.Default.AutoScrobbleCache = value;
        NotifyOfPropertyChange();
      }
    }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    public CacheScrobblerViewModel(IExtendedWindowManager windowManager)
      : base(windowManager, "Cache Scrobbler")
    {
      Scrobbles = new ObservableCollection<DatedScrobbleViewModel>();
      StartupHandling();
    }

    /// <summary>
    /// Handles the auto scrobble.
    /// </summary>
    private async void StartupHandling()
    {
      if (base.CanScrobble)
      {
        await GetCachedScrobbles();
        if (AutoScrobble && CanScrobble)
          await Scrobble();
      }
    }

    /// <summary>
    /// Creates a list with scrobbles that will be scrobbles
    /// with the current configuration.
    /// </summary>
    /// <returns>List with scrobbles.</returns>
    protected override IEnumerable<Scrobble> CreateScrobbles()
    {
      return Scrobbles.Select(vm => new Scrobble(vm.ArtistName, vm.AlbumName, vm.TrackName, vm.Played)
                             { AlbumArtist = vm.AlbumArtist, Duration = vm.Duration });
    }

    /// <summary>
    /// Scrobbles the selected tracks.
    /// </summary>
    /// <returns>Task.</returns>
    public override async Task Scrobble()
    {
      try
      {
        EnableControls = false;
        OnStatusUpdated("Trying to scrobble cached tracks...");
        var response = await Scrobbler.SendCachedScrobblesAsync();

        if (response.Success && response.Status == LastResponseStatus.Successful)
          OnStatusUpdated("Successfully scrobbled cached tracks");
        else
          OnStatusUpdated(string.Format("Error scrobbling cached tracks: {0}", response.Status));
      }
      catch(Exception ex)
      {
        OnStatusUpdated(string.Format("Fatal error while trying to scrobble cached tracks: {0}", ex.Message));
      }
      finally
      {
        await GetCachedScrobbles();
        EnableControls = true;
      }
    }

    /// <summary>
    /// Read the cached srobbles.
    /// </summary>
    public async Task GetCachedScrobbles()
    {
      try
      {
        EnableControls = false;
        Scrobbles.Clear();
        var scrobbles = new ObservableCollection<Scrobble>(await Scrobbler.GetCachedAsync());

        foreach(var s in scrobbles)
        {
          Scrobbles.Add(new DatedScrobbleViewModel(new DatedScrobble(s.TimePlayed.DateTime, s.Track, s.Artist, s.Album, s.AlbumArtist, s.Duration)));
        }
      }
      catch(Exception ex)
      {
        OnStatusUpdated(string.Format("Fatal error getting cached scrobbles: {0}", ex.Message));
      }
      finally
      {
        EnableControls = true;
      }
    }
  }
}