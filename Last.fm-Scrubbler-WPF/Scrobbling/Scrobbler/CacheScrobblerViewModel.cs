using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Scrubbler.Helper;
using Scrubbler.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Scrubbler.Scrobbling.Scrobbler
{
  /// <summary>
  /// ViewModel for the <see cref="CacheScrobblerView"/>
  /// </summary>
  public class CacheScrobblerViewModel : ScrobbleViewModelBase
  {
    #region Properties

    /// <summary>
    /// Gets if the scrobble button is enabled.
    /// </summary>
    public override bool CanScrobble => base.CanScrobble && Scrobbles.Count > 0;

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public override bool CanPreview => Scrobbles.Count > 0;

    /// <summary>
    /// List with the cached scrobbles.
    /// </summary>
    public ObservableCollection<Scrobble> Scrobbles
    {
      get { return _Scrobbles; }
      private set
      {
        _Scrobbles = value;
        NotifyOfPropertyChange();
        NotifyOfPropertyChange(() => CanScrobble);
        NotifyOfPropertyChange(() => CanPreview);
      }
    }
    private ObservableCollection<Scrobble> _Scrobbles;

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
      Scrobbles = new ObservableCollection<Scrobble>();
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
      return Scrobbles;
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
          OnStatusUpdated($"Error scrobbling cached tracks: {response.Status}");
      }
      catch(Exception ex)
      {
        OnStatusUpdated($"Fatal error while trying to scrobble cached tracks: {ex.Message}");
      }
      finally
      {
        await GetCachedScrobbles();
        EnableControls = true;
      }
    }

    /// <summary>
    /// Read the cached scrobbles.
    /// </summary>
    public async Task GetCachedScrobbles()
    {
      try
      {
        EnableControls = false;
        OnStatusUpdated("Trying to get cached scrobbles...");
        Scrobbles = new ObservableCollection<Scrobble>(await Scrobbler.GetCachedAsync());
        OnStatusUpdated($"Successfully got cached scrobbles ({Scrobbles.Count})");
      }
      catch(Exception ex)
      {
        Scrobbles.Clear();
        OnStatusUpdated($"Fatal error getting cached scrobbles: {ex.Message}");
      }
      finally
      {
        EnableControls = true;
      }
    }
  }
}