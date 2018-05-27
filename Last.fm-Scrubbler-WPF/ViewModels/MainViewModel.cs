using Caliburn.Micro;
using Scrubbler.Interfaces;
using Scrubbler.Models;
using Scrubbler.Properties;
using Scrubbler.ViewModels.ExtraFunctions;
using Scrubbler.ViewModels.ScrobbleViewModels;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Scrubbler.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.MainView"/>.
  /// </summary>
  public class MainViewModel : Conductor<Screen>.Collection.OneActive
  {
    #region Properties

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
    /// ViewModel for the <see cref="Views.UserView"/>.
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
    /// The minimum DateTime for all DateTimePickers.
    /// </summary>
    public static DateTime MinimumDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(14));

    #region StatusBar Properties

    /// <summary>
    /// Gets the username displayed in the status bar.
    /// </summary>
    public string StatusBarUsername
    {
      get { return _client.Auth.Authenticated ? _client.Auth.UserSession.Username : "Not logged in."; }
    }

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
      }
    }
    private string _currentStatus;

    #endregion StatusBar Properties

    #endregion Properties

    #region Private Member

    /// <summary>
    /// The client used for all last.fm actions.
    /// </summary>
    private ILastFMClient _client;

    /// <summary>
    /// The api key of this application.
    /// </summary>
    private const string APIKEY = "9d226e3ff2fcd55a42c553fef62a7553";

    /// <summary>
    /// The api secret of this application.
    /// </summary>
    private const string APISECRET = "c779028b480f6c63449c380aeebbbd63";

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
    private IExtendedWindowManager _windowManager;

    /// <summary>
    /// Factory used for creating clients.
    /// </summary>
    private static ILastFMClientFactory _lastFMClientFactory;

    /// <summary>
    /// Factory used for creating scrobblers.
    /// </summary>
    private IScrobblerFactory _scrobblerFactory;

    /// <summary>
    /// FileOperator for interfacing with the hard disk.
    /// </summary>
    private IFileOperator _fileOperator;

    #endregion Private Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="clientFactory">Factory for creating <see cref="ILastFMClient"/>s.</param>
    /// <param name="scrobblerFactory">Factory for creating <see cref="IAuthScrobbler"/>s.</param>
    /// <param name="localFileFactory">Factory for creating <see cref="ILocalFile"/>s.</param>
    /// <param name="fileOperator">FileOperator for interfacing with the hard disk.</param>
    /// <param name="directoryOperator">DirectoryOperator for operating with directories.</param>
    /// <param name="userSerializer">Serializer for <see cref="User"/>s.</param>
    public MainViewModel(IExtendedWindowManager windowManager, ILastFMClientFactory clientFactory, IScrobblerFactory scrobblerFactory, ILocalFileFactory localFileFactory,
                         IFileOperator fileOperator, IDirectoryOperator directoryOperator, ISerializer<User> userSerializer)
    {
      _windowManager = windowManager;
      _lastFMClientFactory = clientFactory;
      _scrobblerFactory = scrobblerFactory;
      _fileOperator = fileOperator;
      TitleString = "Last.fm Scrubbler WPF Beta " + Assembly.GetExecutingAssembly().GetName().Version;
      _client = _lastFMClientFactory.CreateClient(APIKEY, APISECRET);
      SetupViewModels(localFileFactory, directoryOperator, userSerializer);
      CurrentStatus = "Waiting to scrobble...";
    }

    #endregion Construction

    /// <summary>
    /// Shows the general settings.
    /// </summary>
    public void ShowSettings()
    {
      _windowManager.ShowDialog(new GeneralSettingsViewModel());
    }

    #region Setup

    /// <summary>
    /// Creates the ViewModels.
    /// </summary>
    /// <param name="localFileFactory">Factory for creating <see cref="ILocalFile"/>s.</param>
    /// <param name="directoryOperator">DirectoryOperator for operating with directories.</param>
    /// <param name="userSerializer">Serializer for <see cref="User"/>s.</param>
    private void SetupViewModels(ILocalFileFactory localFileFactory, IDirectoryOperator directoryOperator, ISerializer<User> userSerializer)
    {
      UserViewModel = new UserViewModel(_windowManager, _client.Auth, _fileOperator, directoryOperator, userSerializer);
      UserViewModel.LoadLastUser();

      _scrobblerVM = new ScrobblerViewModel(_windowManager, localFileFactory, _fileOperator, _client);
      _scrobblerVM.StatusUpdated += StatusUpdated;
      ActivateItem(_scrobblerVM);

      _extraFunctionsVM = new ExtraFunctionsViewModel(_windowManager, _client.User, _fileOperator);
      _extraFunctionsVM.StatusUpdated += StatusUpdated;
      ActivateItem(_extraFunctionsVM);

      // we do this later to stop a NullReferenceException (we create scrobblers here anyways!)
      UserViewModel.ActiveUserChanged += UserViewModel_ActiveUserChanged;

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
      NotifyOfPropertyChange(() => StatusBarUsername);
      CreateScrobblers();
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
        string dbFile = _client.Auth.UserSession.Username + ".db";

        try
        {
          if (!File.Exists(dbFile))
            File.Create(dbFile);
        }
        catch (Exception ex)
        {
          CurrentStatus = "Error creating cache database. Error: " + ex.Message;
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
    /// <param name="e">Contains the new status.</param>
    private void StatusUpdated(object sender, UpdateStatusEventArgs e)
    {
      CurrentStatus = e.NewStatus;
    }

    /// <summary>
    /// Shows the <see cref="Views.UserView"/>.
    /// </summary>
    public void HyperlinkClicked()
    {
      _windowManager.ShowDialog(UserViewModel);
      NotifyOfPropertyChange(() => StatusBarUsername);
    }

    /// <summary>
    /// Cleanup before exiting.
    /// </summary>
    /// <param name="close">True if the application is closed.</param>
    protected override void OnDeactivate(bool close)
    {
      if(close)
      {
        foreach (IDisposable disposableVM in Items.Where(i => i is IDisposable))
        {
          disposableVM.Dispose();
        }
      }
    }
  }
}