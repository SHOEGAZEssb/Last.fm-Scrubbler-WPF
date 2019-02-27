using Caliburn.Micro;
using DiscogsClient;
using Scrubbler.Helper;
using Scrubbler.Scrobbling.Scrobbler;
using System;
using System.Linq;

namespace Scrubbler.Scrobbling
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
    /// <param name="localFileFactory">Factory used to create <see cref="Scrobbling.Data.ILocalFile"/>s.</param>
    /// <param name="fileOperator">FileOperator used to interface with files.</param>
    /// <param name="lastFMClient">Last.fm client.</param>
    /// <param name="discogsClient">Client used to interact with Discogs.com</param>
    public ScrobblerViewModel(IExtendedWindowManager windowManager, ILocalFileFactory localFileFactory, IFileOperator fileOperator, ILastFMClient lastFMClient,
                              IDiscogsDataBaseClient discogsClient)
    {
      DisplayName = "Scrobbler";
      CreateViewModels(windowManager, localFileFactory, fileOperator, lastFMClient, discogsClient);
    }

    public void UpdateScrobblers(IUserScrobbler scrobbler)
    {
      foreach(var vm in Items)
      {
        vm.Scrobbler = scrobbler;
      }
    }

    /// <summary>
    /// Creates the ViewModels.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="localFileFactory">Factory used to create <see cref="Data.ILocalFile"/>s.</param>
    /// <param name="fileOperator">FileOperator used to interface with files.</param>
    /// <param name="lastFMClient">Last.fm client.</param>
    /// <param name="discogsClient">Client used to interact with Discogs.com</param>
    private void CreateViewModels(IExtendedWindowManager windowManager, ILocalFileFactory localFileFactory, IFileOperator fileOperator, ILastFMClient lastFMClient,
                                  IDiscogsDataBaseClient discogsClient)
    {
      var manualScrobbleViewModel = new ManualScrobbleViewModel(windowManager);
      manualScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var friendScrobbleViewModel = new FriendScrobbleViewModel(windowManager, lastFMClient.User);
      friendScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var databaseScrobbleViewModel = new DatabaseScrobbleViewModel(windowManager, lastFMClient.Artist, lastFMClient.Album, discogsClient);
      databaseScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var csvScrobbleViewModel = new CSVScrobbleViewModel(windowManager, new CSVTextFieldParserFactory(), fileOperator);
      csvScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var fileScrobbleViewModel = new FileScrobbleViewModel(windowManager, localFileFactory, fileOperator);
      fileScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var mediaPlayerDatabaseScrobbleViewModel = new MediaPlayerDatabaseScrobbleViewModel(windowManager);
      mediaPlayerDatabaseScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var iTunesScrobbleVM = new ITunesScrobbleViewModel(windowManager, lastFMClient.Track, lastFMClient.Album, lastFMClient.Artist);
      iTunesScrobbleVM.StatusUpdated += Scrobbler_StatusUpdated;
      // todo: put back in once local api is working again
      //var spotifyScrobbleVM = new SpotifyScrobbleViewModel(windowManager, lastFMClient.Track, lastFMClient.Album, lastFMClient.Auth);
      //spotifyScrobbleVM.StatusUpdated += Scrobbler_StatusUpdated;
      var setlistFMScrobbleVM = new SetlistFMScrobbleViewModel(windowManager, lastFMClient.Artist);
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
      //ActivateItem(spotifyScrobbleVM);
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
      foreach (IDisposable disposableVM in Items.Where(i => i is IDisposable))
      {
        disposableVM.Dispose();
      }
    }
  }
}