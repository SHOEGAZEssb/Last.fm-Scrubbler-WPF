using Caliburn.Micro;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Scrobblers;
using Last.fm_Scrubbler_WPF.Views;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="MainView"/>.
  /// </summary>
  class MainViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// The client used for all last.fm actions.
    /// </summary>
    public static LastfmClient Client
    {
      get { return _client; }
      private set
      {
        _client = value;
      }
    }
    private static LastfmClient _client;

    /// <summary>
    /// Scrobbler used to scrobble tracks.
    /// </summary>
    public static Scrobbler Scrobbler
    {
      get { return _scrobbler; }
      private set
      {
        _scrobbler = value;
        ClientAuthChanged?.Invoke(null, EventArgs.Empty);
      }
    }
    private static Scrobbler _scrobbler;

    /// <summary>
    /// ViewModel for the <see cref="SelectUserView"/>.
    /// </summary>
    public UserViewModel UserViewModel
    {
      get { return _userViewModel; }
      private set
      {
        _userViewModel = value;
        NotifyOfPropertyChange(() => UserViewModel);
      }
    }
    private UserViewModel _userViewModel;

    /// <summary>
    /// Event that triggers when the client authentication changes.
    /// </summary>
    public static EventHandler<EventArgs> ClientAuthChanged;

    /// <summary>
    /// The minimum DateTime for all DateTimePickers.
    /// </summary>
    public static DateTime MinimumDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(14));

    /// <summary>
    /// The ViewModel for the <see cref="ManualScrobbleView"/>.
    /// </summary>
    public ManualScrobbleViewModel ManualScrobbleViewModel
    {
      get { return _manualScrobbleViewModel; }
      private set
      {
        _manualScrobbleViewModel = value;
        NotifyOfPropertyChange(() => ManualScrobbleViewModel);
      }
    }
    private ManualScrobbleViewModel _manualScrobbleViewModel;

    /// <summary>
    /// The ViewModel for the <see cref="FriendScrobbleView"/>.
    /// </summary>
    public FriendScrobbleViewModel FriendScrobbleViewModel
    {
      get { return _friendScrobbleViewModel; }
      private set
      {
        _friendScrobbleViewModel = value;
        NotifyOfPropertyChange(() => FriendScrobbleViewModel);
      }
    }
    private FriendScrobbleViewModel _friendScrobbleViewModel;

    /// <summary>
    /// The ViewModel for the <see cref="DatabaseScrobbleView"/>.
    /// </summary>
    public DatabaseScrobbleViewModel DatabaseScrobbleViewModel
    {
      get { return _databaseScrobbleViewModel; }
      private set
      {
        _databaseScrobbleViewModel = value;
        NotifyOfPropertyChange(() => DatabaseScrobbleViewModel);
      }
    }
    private DatabaseScrobbleViewModel _databaseScrobbleViewModel;

    /// <summary>
    /// The ViewModel for the <see cref="CSVScrobbleView"/>.
    /// </summary>
    public CSVScrobbleViewModel CSVScrobbleViewModel
    {
      get { return _csvScrobbleViewModel; }
      private set
      {
        _csvScrobbleViewModel = value;
        NotifyOfPropertyChange(() => CSVScrobbleViewModel);
      }
    }
    private CSVScrobbleViewModel _csvScrobbleViewModel;

    /// <summary>
    /// ViewModel for the <see cref="FileScrobbleView"/>.
    /// </summary>
    public FileScrobbleViewModel FileScrobbleViewModel
    {
      get { return _fileScrobbleViewModel; }
      private set
      {
        _fileScrobbleViewModel = value;
        NotifyOfPropertyChange(() => FileScrobbleViewModel);
      }
    }
    private FileScrobbleViewModel _fileScrobbleViewModel;

    /// <summary>
    /// ViewModel for the <see cref="MediaPlayerDatabaseScrobbleView"/>.
    /// </summary>
    public MediaPlayerDatabaseScrobbleViewModel MediaPlayerDatabaseScrobbleViewModel
    {
      get { return _mediaPlayerDatabaseScrobbleViewModel; }
      private set
      {
        _mediaPlayerDatabaseScrobbleViewModel = value;
        NotifyOfPropertyChange(() => MediaPlayerDatabaseScrobbleViewModel);
      }
    }
    private MediaPlayerDatabaseScrobbleViewModel _mediaPlayerDatabaseScrobbleViewModel;

    /// <summary>
    /// ViewModel for the <see cref="ITunesScrobbleView"/>.
    /// </summary>
    public ITunesScrobbleViewModel ITunesScrobbleVM
    {
      get { return _iTunesScrobbleVM; }
      private set
      {
        _iTunesScrobbleVM = value;
        NotifyOfPropertyChange(() => ITunesScrobbleVM);
      }
    }
    private ITunesScrobbleViewModel _iTunesScrobbleVM;

    #region StatusBar Properties

    /// <summary>
    /// Gets the username displayed in the status bar.
    /// </summary>
    public string StatusBarUsername
    {
      get { return Client.Auth.Authenticated ? Client.Auth.UserSession.Username : "Not logged in."; }
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
        NotifyOfPropertyChange(() => CurrentStatus);
      }
    }
    private string _currentStatus;

    #endregion StatusBar Properties

    #endregion Properties

    #region Private Member

    /// <summary>
    /// The api key of this application.
    /// </summary>
    private const string APIKEY = "9d226e3ff2fcd55a42c553fef62a7553";

    /// <summary>
    /// The api secret of this application.
    /// </summary>
    private const string APISECRET = "c779028b480f6c63449c380aeebbbd63";

    #endregion Private Member

    /// <summary>
    /// Constructor.
    /// </summary>
    public MainViewModel()
    {
      CreateNewClient();
      UserViewModel = new UserViewModel();
      UserViewModel.ActiveUserChanged += UserViewModel_ActiveUserChanged;
      UserViewModel.LoadLastUser();
      ManualScrobbleViewModel = new ManualScrobbleViewModel();
      ManualScrobbleViewModel.StatusUpdated += StatusUpdated;
      FriendScrobbleViewModel = new FriendScrobbleViewModel();
      FriendScrobbleViewModel.StatusUpdated += StatusUpdated;
      DatabaseScrobbleViewModel = new DatabaseScrobbleViewModel();
      DatabaseScrobbleViewModel.StatusUpdated += StatusUpdated;
      CSVScrobbleViewModel = new CSVScrobbleViewModel();
      CSVScrobbleViewModel.StatusUpdated += StatusUpdated;
      FileScrobbleViewModel = new FileScrobbleViewModel();
      FileScrobbleViewModel.StatusUpdated += StatusUpdated;
      MediaPlayerDatabaseScrobbleViewModel = new MediaPlayerDatabaseScrobbleViewModel();
      MediaPlayerDatabaseScrobbleViewModel.StatusUpdated += StatusUpdated;
      ITunesScrobbleVM = new ITunesScrobbleViewModel();
      ITunesScrobbleVM.StatusUpdated += StatusUpdated;
    }

    /// <summary>
    /// Creates a new <see cref="LastfmClient"/>.
    /// </summary>
    internal static void CreateNewClient()
    {
      Client = new LastfmClient(APIKEY, APISECRET);
      ClientAuthChanged?.Invoke(null, EventArgs.Empty);
    }

    /// <summary>
    /// Triggers when the <see cref="UserViewModel.ActiveUser"/> changes.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void UserViewModel_ActiveUserChanged(object sender, EventArgs e)
    {
      NotifyOfPropertyChange(() => StatusBarUsername);
      Scrobbler = new Scrobbler(Client.Auth);
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
    /// Shows the <see cref="LoginView"/>.
    /// </summary>
    public void HyperlinkClicked()
    {
      SelectUserView suv = new SelectUserView();
      suv.DataContext = UserViewModel;
      suv.ShowDialog();

      NotifyOfPropertyChange(() => StatusBarUsername);
    }

    /// <summary>
    /// Dispose iTunes com object before exit.
    /// </summary>
    public void MainView_Closing()
    {
      ITunesScrobbleVM.Dispose();
    }
  }
}