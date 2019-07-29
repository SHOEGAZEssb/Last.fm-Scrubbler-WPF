using Caliburn.Micro;
using DiscogsClient;
using Octokit;
using Scrubbler.Configuration;
using Scrubbler.ExtraFunctions;
using Scrubbler.Helper;
using Scrubbler.Login;
using Scrubbler.Scrobbling;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Scrubbler
{
  /// <summary>
  /// ViewModel for the <see cref="MainView"/>.
  /// </summary>
  public class MainViewModel : Conductor<Screen>.Collection.OneActive, IDisposable
  {
    #region Properties

    /// <summary>
    /// The minimum DateTime for all DateTimePickers.
    /// </summary>
    public static DateTime MinimumDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(14));

    /// <summary>
    /// String containing application name and version.
    /// </summary>
    public string TitleString
    {
      get { return _titleString; }
      private set
      {
        _titleString = value;
        NotifyOfPropertyChange();
      }
    }
    private string _titleString;

    /// <summary>
    /// ViewModel for the <see cref="UserView"/>.
    /// </summary>
    public UserViewModel UserViewModel
    {
      get { return _userViewModel; }
      private set
      {
        _userViewModel = value;
        NotifyOfPropertyChange();
      }
    }
    private UserViewModel _userViewModel;

    /// <summary>
    /// Gets the current status displayed in the status bar.
    /// </summary>
    public string CurrentStatus
    {
      get { return _currentStatus; }
      private set
      {
        _currentStatus = value;
        NotifyOfPropertyChange();

        _logger?.Log(CurrentStatus);
      }
    }
    private string _currentStatus;

    #endregion Properties

    #region Member

    /// <summary>
    /// The client used for all last.fm actions.
    /// </summary>
    private readonly ILastFMClient _client;

    /// <summary>
    /// ViewModel managing all scrobble ViewModels.
    /// </summary>
    private ScrobblerViewModel _scrobblerVM;

    /// <summary>
    /// ViewModel managing all extra function ViewModels.
    /// </summary>
    private ExtraFunctionsViewModel _extraFunctionsVM;

    /// <summary>
    /// Window manager used to display dialogs etc.
    /// </summary>
    private readonly IExtendedWindowManager _windowManager;

    /// <summary>
    /// Factory used for creating scrobblers.
    /// </summary>
    private readonly IScrobblerFactory _scrobblerFactory;

    /// <summary>
    /// FileOperator for interfacing with the hard disk.
    /// </summary>
    private readonly IFileOperator _fileOperator;

    /// <summary>
    /// Logger used to log status messages.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// ViewModel for the general settings.
    /// </summary>
    private GeneralSettingsViewModel _generalSettingsVM;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="client">Factory for creating <see cref="ILastFMClient"/>s.</param>
    /// <param name="scrobblerFactory">Factory for creating <see cref="IAuthScrobbler"/>s.</param>
    /// <param name="localFileFactory">Factory for creating <see cref="Scrobbling.Data.ILocalFile"/>s.</param>
    /// <param name="fileOperator">FileOperator for interfacing with the hard disk.</param>
    /// <param name="directoryOperator">DirectoryOperator for operating with directories.</param>
    /// <param name="serializer">Serializer for <see cref="User"/>s.</param>
    /// <param name="logger">Logger used to log status messages.</param>
    /// <param name="gitHubClient">GitHub client to check for updates.</param>
    /// <param name="processManager">ProcessManager for working with processor functions.</param>
    /// <param name="discogsClient">Client used to interact with Discogs.com</param>
    public MainViewModel(IExtendedWindowManager windowManager, ILastFMClient client, IScrobblerFactory scrobblerFactory, ILocalFileFactory localFileFactory,
                         IFileOperator fileOperator, IDirectoryOperator directoryOperator, ISerializer serializer, ILogger logger, IGitHubClient gitHubClient,
                         IProcessManager processManager, IDiscogsDataBaseClient discogsClient)
    {
      _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
      _client = client ?? throw new ArgumentNullException(nameof(client));
      _scrobblerFactory = scrobblerFactory ?? throw new ArgumentNullException(nameof(scrobblerFactory));
      _fileOperator = fileOperator ?? throw new ArgumentNullException(nameof(fileOperator));
      _logger = logger;
      SetupViewModels(localFileFactory, directoryOperator, serializer, gitHubClient, processManager, discogsClient);
      TitleString = $"Last.fm Scrubbler WPF Beta {Assembly.GetExecutingAssembly().GetName().Version}";
      CurrentStatus = "Waiting to scrobble...";
    }

    #endregion Construction

    /// <summary>
    /// Shows the general settings.
    /// </summary>
    public void ShowSettings()
    {
      _windowManager.ShowDialog(_generalSettingsVM);
    }

    /// <summary>
    /// Shows the <see cref="UserView"/>.
    /// </summary>
    public void ShowUserView()
    {
      _windowManager.ShowDialog(UserViewModel);
    }

    /// <summary>
    /// Cleanup before exiting.
    /// </summary>
    /// <param name="close">True if the application is closed.</param>
    protected override void OnDeactivate(bool close)
    {
      // we need to override this because the default
      // implementation disposes the items on deactivation.
      // but we want to dispose manually.
    }

    #region Setup

    /// <summary>
    /// Creates the ViewModels.
    /// </summary>
    /// <param name="localFileFactory">Factory for creating <see cref="Scrobbling.Data.ILocalFile"/>s.</param>
    /// <param name="directoryOperator">DirectoryOperator for operating with directories.</param>
    /// <param name="serializer">Serializer for <see cref="User"/>s.</param>
    /// <param name="gitHubClient">GitHub client to check for updates.</param>
    /// <param name="processManager">ProcessManager for working with processor functions.</param>
    /// <param name="discogsClient">Client used to interact with Discogs.com</param>
    private void SetupViewModels(ILocalFileFactory localFileFactory, IDirectoryOperator directoryOperator, ISerializer serializer, IGitHubClient gitHubClient,
                                 IProcessManager processManager, IDiscogsDataBaseClient discogsClient)
    {
      UserViewModel = new UserViewModel(_windowManager, _client.Auth, _fileOperator, directoryOperator, serializer);

      _generalSettingsVM = new GeneralSettingsViewModel(_windowManager, gitHubClient, processManager);

      _scrobblerVM = new ScrobblerViewModel(_windowManager, localFileFactory, _fileOperator, _client, discogsClient);
      _scrobblerVM.StatusUpdated += StatusUpdated;
      ActivateItem(_scrobblerVM);

      _extraFunctionsVM = new ExtraFunctionsViewModel(_windowManager, _client.User, _fileOperator);
      _extraFunctionsVM.StatusUpdated += StatusUpdated;
      ActivateItem(_extraFunctionsVM);

      // we do this later to stop a NullReferenceException (we create scrobblers here anyways!)
      UserViewModel.ActiveUserChanged += UserViewModel_ActiveUserChanged;

      if(UserViewModel.ActiveUser != null)
        CreateScrobblers();

      // should be active
      ActivateItem(_scrobblerVM);
    }

    #endregion Setup

    /// <summary>
    /// Triggers when the <see cref="UserViewModel.ActiveUser"/> changes.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void UserViewModel_ActiveUserChanged(object sender, EventArgs e)
    {
      if (UserViewModel.ActiveUser != null)
        CreateScrobblers();
      else
        _scrobblerVM.UpdateScrobblers(null);
    }

    /// <summary>
    /// Recreates the scrobblers of the ScrobbleViewModels.
    /// </summary>
    private void CreateScrobblers()
    {
      IAuthScrobbler scrobbler;
      IAuthScrobbler cachingScrobbler;
      if (_client.Auth.UserSession != null)
      {
        var dbFile = $"{_client.Auth.UserSession.Username}.db";

        try
        {
          if (!File.Exists(dbFile))
            File.Create(dbFile);
        }
        catch (Exception ex)
        {
          CurrentStatus = $"Error creating cache database. Error: {ex.Message}";
        }

        scrobbler = _scrobblerFactory.CreateScrobbler(_client.Auth);
        cachingScrobbler = _scrobblerFactory.CreateSQLiteScrobbler(_client.Auth, dbFile);
      }
      else
      {
        scrobbler = null;
        cachingScrobbler = null;
      }

      _scrobblerVM.UpdateScrobblers(_scrobblerFactory.CreateUserScrobbler(UserViewModel.ActiveUser, scrobbler, cachingScrobbler));
    }

    /// <summary>
    /// Updates the status.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">EventArgs containing the new status message.</param>
    private void StatusUpdated(object sender, UpdateStatusEventArgs e)
    {
      CurrentStatus = e.NewStatus;
    }

    /// <summary>
    /// Disposes resources.
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