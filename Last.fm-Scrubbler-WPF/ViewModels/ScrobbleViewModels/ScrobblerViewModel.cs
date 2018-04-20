using Caliburn.Micro;
using Scrubbler.Interfaces;
using Scrubbler.Models;
using System;

namespace Scrubbler.ViewModels.ScrobbleViewModels
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

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="localFileFactory">Factory used to create <see cref="ILocalFile"/>s.</param>
    /// <param name="fileOperator">FileOperator used to interface with files.</param>
    public ScrobblerViewModel(IExtendedWindowManager windowManager, ILocalFileFactory localFileFactory, IFileOperator fileOperator)
    {
      DisplayName = "Scrobbler";
      CreateViewModels(windowManager, localFileFactory, fileOperator);
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
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="localFileFactory">Factory used to create <see cref="ILocalFile"/>s.</param>
    /// <param name="fileOperator">FileOperator used to interface with files.</param>
    private void CreateViewModels(IExtendedWindowManager windowManager, ILocalFileFactory localFileFactory, IFileOperator fileOperator)
    {
      var manualScrobbleViewModel = new ManualScrobbleViewModel(windowManager);
      manualScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var friendScrobbleViewModel = new FriendScrobbleViewModel(windowManager, MainViewModel.Client.User);
      friendScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var databaseScrobbleViewModel = new DatabaseScrobbleViewModel(windowManager, MainViewModel.Client.Artist, MainViewModel.Client.Album);
      databaseScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var csvScrobbleViewModel = new CSVScrobbleViewModel(windowManager, new CSVTextFieldParserFactory(), fileOperator);
      csvScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var fileScrobbleViewModel = new FileScrobbleViewModel(windowManager, localFileFactory, fileOperator);
      fileScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var mediaPlayerDatabaseScrobbleViewModel = new MediaPlayerDatabaseScrobbleViewModel(windowManager);
      mediaPlayerDatabaseScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var iTunesScrobbleVM = new ITunesScrobbleViewModel(windowManager);
      iTunesScrobbleVM.StatusUpdated += Scrobbler_StatusUpdated;
      var spotifyScrobbleVM = new SpotifyScrobbleViewModel(windowManager);
      spotifyScrobbleVM.StatusUpdated += Scrobbler_StatusUpdated;
      var setlistFMScrobbleVM = new SetlistFMScrobbleViewModel(windowManager);
      setlistFMScrobbleVM.StatusUpdated += Scrobbler_StatusUpdated;
      var cacheScrobblerVM = new CacheScrobblerViewModel(windowManager);
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