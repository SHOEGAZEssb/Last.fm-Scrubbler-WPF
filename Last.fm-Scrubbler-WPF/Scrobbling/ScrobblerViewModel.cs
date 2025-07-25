using DiscogsClient;
using Scrubbler.Helper;
using Scrubbler.Scrobbling.Scrobbler;
using ScrubblerLib;
using ScrubblerLib.Helper;
using ScrubblerLib.Helper.FileParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrubbler.Scrobbling
{
  /// <summary>
  /// ViewModel managing all Scrobbler ViewModels.
  /// </summary>
  class ScrobblerViewModel : TabViewModel, IDisposable
  {
    #region Properties

    /// <summary>
    /// The available scrobblers.
    /// </summary>
    public IEnumerable<ScrobbleViewModelBase> Scrobblers { get; }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="localFileFactory">Factory used to create <see cref="ScrubblerLib.Data.ILocalFile"/>s.</param>
    /// <param name="fileOperator">FileOperator used to interface with files.</param>
    /// <param name="lastFMClient">Last.fm client.</param>
    /// <param name="discogsClient">Client used to interact with Discogs.com</param>
    /// <param name="fileParserFactory">Factory for creating <see cref="IFileParser"/></param>
    public ScrobblerViewModel(IExtendedWindowManager windowManager, ILocalFileFactory localFileFactory, IFileOperator fileOperator, ILastFMClient lastFMClient,
                              IDiscogsDataBaseClient discogsClient, IFileParserFactory fileParserFactory, ISerializer serializer)
      : base("Scrobbler")
    {
      Scrobblers = CreateViewModels(windowManager, localFileFactory, fileOperator, lastFMClient, discogsClient, fileParserFactory, serializer);
    }

    /// <summary>
    /// Update the scrobbler used by the ScrobbleViewModels.
    /// </summary>
    /// <param name="scrobbler">Scrobbler to use.</param>
    public void UpdateScrobblers(IUserScrobbler scrobbler)
    {
      foreach(var vm in Scrobblers)
      {
        vm.Scrobbler = scrobbler;
      }
    }

    /// <summary>
    /// Creates the ViewModels.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="localFileFactory">Factory used to create <see cref="ScrubblerLib.Data.ILocalFile"/>s.</param>
    /// <param name="fileOperator">FileOperator used to interface with files.</param>
    /// <param name="lastFMClient">Last.fm client.</param>
    /// <param name="discogsClient">Client used to interact with Discogs.com</param>
    /// <param name="fileParserFactory">Factory for creating <see cref="IFileParser"/></param>
    private ScrobbleViewModelBase[] CreateViewModels(IExtendedWindowManager windowManager, ILocalFileFactory localFileFactory, IFileOperator fileOperator, ILastFMClient lastFMClient,
                                  IDiscogsDataBaseClient discogsClient, IFileParserFactory fileParserFactory, ISerializer serializer)
    {
      var manualScrobbleViewModel = new ManualScrobbleViewModel(windowManager);
      manualScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var friendScrobbleViewModel = new FriendScrobbleViewModel(windowManager, lastFMClient.User);
      friendScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var databaseScrobbleViewModel = new DatabaseScrobbleViewModel(windowManager, lastFMClient.Artist, lastFMClient.Album, discogsClient);
      databaseScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var csvScrobbleViewModel = new FileParseScrobbleViewModel(windowManager, fileParserFactory, fileOperator, serializer);
      csvScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var fileScrobbleViewModel = new FileScrobbleViewModel(windowManager, localFileFactory, fileOperator);
      fileScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var mediaPlayerDatabaseScrobbleViewModel = new MediaPlayerDatabaseScrobbleViewModel(windowManager);
      mediaPlayerDatabaseScrobbleViewModel.StatusUpdated += Scrobbler_StatusUpdated;
      var iTunesScrobbleVM = new ITunesScrobbleViewModel(windowManager, lastFMClient.Track, lastFMClient.Album, lastFMClient.Artist);
      iTunesScrobbleVM.StatusUpdated += Scrobbler_StatusUpdated;
      var appleMusicScrobbleVM = new AppleMusicScrobbleViewModel(windowManager, lastFMClient.Track, lastFMClient.Album, lastFMClient.Artist);
      appleMusicScrobbleVM.StatusUpdated += Scrobbler_StatusUpdated;
      // todo: put back in once local api is working again
      //var spotifyScrobbleVM = new SpotifyScrobbleViewModel(windowManager, lastFMClient.Track, lastFMClient.Album, lastFMClient.Auth);
      //spotifyScrobbleVM.StatusUpdated += Scrobbler_StatusUpdated;
      var setlistFMScrobbleVM = new SetlistFMScrobbleViewModel(windowManager, lastFMClient.Artist);
      setlistFMScrobbleVM.StatusUpdated += Scrobbler_StatusUpdated;
      //var cacheScrobblerVM = new CacheScrobblerViewModel(windowManager);
      //cacheScrobblerVM.StatusUpdated += Scrobbler_StatusUpdated;

      return new ScrobbleViewModelBase[] { manualScrobbleViewModel, friendScrobbleViewModel, databaseScrobbleViewModel, csvScrobbleViewModel,
                                           fileScrobbleViewModel, mediaPlayerDatabaseScrobbleViewModel, iTunesScrobbleVM, appleMusicScrobbleVM, setlistFMScrobbleVM };
    }

    /// <summary>
    /// Fires the StatusUpdated event.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">EventArgs containing the new status message.</param>
    private void Scrobbler_StatusUpdated(object sender, UpdateStatusEventArgs e)
    {
      OnStatusUpdated(e.NewStatus);
    }

    /// <summary>
    /// Disposes all ViewModels that need disposing.
    /// </summary>
    public void Dispose()
    {
      foreach (IDisposable disposableVM in Scrobblers.Where(i => i is IDisposable).Cast<IDisposable>())
      {
        disposableVM.Dispose();
      }
    }
  }
}