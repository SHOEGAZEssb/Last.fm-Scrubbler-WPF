using Caliburn.Micro;
using Last.fm_Scrubbler_WPF.Interfaces;
using Last.fm_Scrubbler_WPF.Models;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels.ScrobbleViewModels
{
  /// <summary>
  /// ViewModel managing all Scrobbler ViewModels.
  /// </summary>
  class ScrobblerViewModel : Conductor<ScrobbleViewModelBase>.Collection.OneActive, IDisposable
  {
    #region Properties

    /// <summary>
    /// Event that triggers when the status of a ViewModel changes.
    /// </summary>
    public event EventHandler<UpdateStatusEventArgs> StatusUpdated;

    #endregion Properties

    #region Member

    private IWindowManager _windowManager;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    public ScrobblerViewModel(IWindowManager windowManager)
    {
      DisplayName = "Scrobbler";
      _windowManager = windowManager;
      CreateViewModels();
    }

    /// <summary>
    /// Updates the scrobblers of the ViewModels.
    /// </summary>
    /// <param name="scrobbler">Normal scrobbler.</param>
    /// <param name="cachingScrobbler">Caching scrobbler.</param>
    public void UpdateScrobblers(IAuthScrobbler scrobbler, IAuthScrobbler cachingScrobbler)
    {
      foreach(var vm in Items)
      {
        if (vm is INeedCachingScrobbler)
          vm.Scrobbler = cachingScrobbler;
        else
          vm.Scrobbler = scrobbler;
      }
    }

    /// <summary>
    /// Creates the ViewModels.
    /// </summary>
    private void CreateViewModels()
    {
      var manualScrobbleViewModel = new ManualScrobbleViewModel(_windowManager);
      manualScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var friendScrobbleViewModel = new FriendScrobbleViewModel(_windowManager, MainViewModel.Client.User);
      friendScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var databaseScrobbleViewModel = new DatabaseScrobbleViewModel(_windowManager);
      databaseScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var csvScrobbleViewModel = new CSVScrobbleViewModel(_windowManager, new CSVTextFieldParserFactory());
      csvScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var fileScrobbleViewModel = new FileScrobbleViewModel(_windowManager);
      fileScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var mediaPlayerDatabaseScrobbleViewModel = new MediaPlayerDatabaseScrobbleViewModel(_windowManager);
      mediaPlayerDatabaseScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var iTunesScrobbleVM = new ITunesScrobbleViewModel(_windowManager);
      iTunesScrobbleVM.StatusUpdated += Scrobbler_StatusUpdated;
      var spotifyScrobbleVM = new SpotifyScrobbleViewModel(_windowManager);
      spotifyScrobbleVM.StatusUpdated += Scrobbler_StatusUpdated;
      var setlistFMScrobbleVM = new SetlistFMScrobbleViewModel(_windowManager);
      setlistFMScrobbleVM.StatusUpdated += Scrobbler_StatusUpdated;
      var cacheScrobblerVM = new CacheScrobblerViewModel(_windowManager);
      cacheScrobblerVM.StatusUpdated += Scrobbler_StatusUpdated;

      ActivateItem(manualScrobbleViewModel);
      ActivateItem(friendScrobbleViewModel);
      ActivateItem(databaseScrobbleViewModel);
      ActivateItem(csvScrobbleViewModel);
      ActivateItem(fileScrobbleViewModel);
      ActivateItem(mediaPlayerDatabaseScrobbleViewModel);
      ActivateItem(iTunesScrobbleVM);
      ActivateItem(spotifyScrobbleVM);
      ActivateItem(setlistFMScrobbleVM);
      ActivateItem(cacheScrobblerVM);

      // this one should be selected
      ActivateItem(manualScrobbleViewModel);
    }

    /// <summary>
    /// Fires the <see cref="StatusUpdated"/> event.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">New status message.</param>
    private void Scrobbler_StatusUpdated(object sender, UpdateStatusEventArgs e)
    {
      StatusUpdated?.Invoke(this, e);
    }

    /// <summary>
    /// Disposes all ViewModels that need disposing.
    /// </summary>
    public void Dispose()
    {
      foreach(var vm in Items)
      {
        if (vm is IDisposable d)
          d.Dispose();
      }
    }
  }
}