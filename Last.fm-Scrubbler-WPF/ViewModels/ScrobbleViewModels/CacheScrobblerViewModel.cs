using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Interfaces;
using Last.fm_Scrubbler_WPF.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF.ViewModels.ScrobbleViewModels
{
  public class CacheScrobblerViewModel : ScrobbleViewModelBase
  {
    #region Properties

    public override bool CanScrobble => MainViewModel.Client.Auth.Authenticated && CachedScrobbles.Count > 0 && EnableControls;

    public override bool CanPreview => CachedScrobbles.Count > 0 && EnableControls;

    public override bool EnableControls
    {
      get { return _enableControls; }
      protected set
      {
        _enableControls = value;
        NotifyOfPropertyChange(() => EnableControls);
        NotifyOfPropertyChange(() => CanScrobble);
        NotifyOfPropertyChange(() => CanPreview);
      }
    }

    /// <summary>
    /// List of cached scrobbles.
    /// </summary>
    public ObservableCollection<Scrobble> CachedScrobbles
    {
      get { return _cachedScrobbles; }
      private set
      {
        _cachedScrobbles = value;
        NotifyOfPropertyChange(() => CachedScrobbles);
        NotifyOfPropertyChange(() => CanScrobble);
        NotifyOfPropertyChange(() => CanPreview);
      }
    }
    private ObservableCollection<Scrobble> _cachedScrobbles;

    /// <summary>
    /// If true, tries to scrobble the cache at application startup.
    /// </summary>
    public bool AutoScrobble
    {
      get { return Settings.Default.AutoScrobbleCache; }
      set
      {
        Settings.Default.AutoScrobbleCache = value;
        Settings.Default.Save();
        NotifyOfPropertyChange(() => AutoScrobble);
      }
    }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    public CacheScrobblerViewModel(IWindowManager windowManager, IAuthScrobbler scrobbler)
      : base(windowManager, scrobbler)
    {
      CachedScrobbles = new ObservableCollection<Scrobble>();
      StartupHandling();
    }

    /// <summary>
    /// Handles the auto scrobble.
    /// </summary>
    private async void StartupHandling()
    {
      if (MainViewModel.Client.Auth.Authenticated)
      {
        await GetCachedScrobbles();
        if (AutoScrobble && CanScrobble)
          await Scrobble();
      }
    }

    protected override IEnumerable<Scrobble> CreateScrobbles()
    {
      return CachedScrobbles;
    }

    public override async Task Scrobble()
    {
      try
      {
        EnableControls = false;
        OnStatusUpdated("Trying to scrobble cached tracks...");
        var response = await Scrobbler.SendCachedScrobblesAsync();

        if (response.Success && response.Status == IF.Lastfm.Core.Api.Enums.LastResponseStatus.Successful)
          OnStatusUpdated("Successfully scrobbled cached tracks");
        else
          OnStatusUpdated("Error scrobbling cached tracks: " + response.Status);
      }
      catch(Exception ex)
      {
        OnStatusUpdated("Fatal error while trying to scrobble cached tracks: " + ex.Message);
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
        CachedScrobbles = new ObservableCollection<Scrobble>(await Scrobbler.GetCachedAsync());
      }
      catch(Exception ex)
      {
        OnStatusUpdated("Fatal error getting cached scrobbles: " + ex.Message);
      }
      finally
      {
        EnableControls = true;
      }
    }
  }
}