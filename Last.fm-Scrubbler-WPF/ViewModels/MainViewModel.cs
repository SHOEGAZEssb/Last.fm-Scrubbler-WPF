using Caliburn.Micro;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Scrobblers;
using IF.Lastfm.SQLite;
using Last.fm_Scrubbler_WPF.ViewModels.ExtraFunctions;
using Last.fm_Scrubbler_WPF.ViewModels.ScrobbleViewModels;
using Last.fm_Scrubbler_WPF.Views;
using Last.fm_Scrubbler_WPF.Views.ExtraFunctions;
using Last.fm_Scrubbler_WPF.Views.ScrobbleViews;
using System;
using System.IO;
using System.Reflection;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="MainView"/>.
  /// </summary>
  class MainViewModel : PropertyChangedBase
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
        NotifyOfPropertyChange(() => TitleString);
      }
    }
    private string _titleString;

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
    public static IScrobbler Scrobbler
    {
      get { return _scrobbler; }
      private set
      {
        _scrobbler = value;
        ClientAuthChanged?.Invoke(null, EventArgs.Empty);
      }
    }
    private static IScrobbler _scrobbler;

    /// <summary>
    /// Scrobbler used to scrobble tracks.
    /// </summary>
    public static IScrobbler CachingScrobbler
    {
      get { return _cachingScrobbler; }
      private set
      {
        _cachingScrobbler = value;
        ClientAuthChanged?.Invoke(null, EventArgs.Empty);
      }
    }
    private static IScrobbler _cachingScrobbler;

    /// <summary>
    /// ViewModel for the <see cref="UserView"/>.
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

    #region ScrobbleViewModels

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
    /// ViewModel for the iTunes <see cref="MediaPlayerScrobbleControl"/>.
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

    /// <summary>
    /// ViewModel for the Spotify <see cref="MediaPlayerScrobbleControl"/>.
    /// </summary>
    public SpotifyScrobbleViewModel SpotifyScrobbleVM
    {
      get { return _spotifyScrobbleVM; }
      private set
      {
        _spotifyScrobbleVM = value;
        NotifyOfPropertyChange(() => SpotifyScrobbleVM);
      }
    }
    private SpotifyScrobbleViewModel _spotifyScrobbleVM;

    /// <summary>
    /// ViewModel for the <see cref="SetlistFMScrobbleViewModel"/>.
    /// </summary>
    public SetlistFMScrobbleViewModel SetlistFMScrobbleVM
    {
      get { return _setlistFMScrobbleVM; }
      private set
      {
        _setlistFMScrobbleVM = value;
        NotifyOfPropertyChange(() => SetlistFMScrobbleVM);
      }
    }
    private SetlistFMScrobbleViewModel _setlistFMScrobbleVM;

    /// <summary>
    /// ViewModel for the <see cref="CacheScrobblerViewModel"/>.
    /// </summary>
    public CacheScrobblerViewModel CacheScrobblerVM
    {
      get { return _cacheScrobblerVM; }
      private set
      {
        _cacheScrobblerVM = value;
        NotifyOfPropertyChange(() => CacheScrobblerVM);
      }
    }
    private CacheScrobblerViewModel _cacheScrobblerVM;

    #endregion ScrobbleViewModels

    #region ExtraViewModels

    /// <summary>
    /// ViewModel for the <see cref="PasteYourTasteView"/>.
    /// </summary>
    public PasteYourTasteViewModel PasteYourTasteVM
    {
      get { return _pasteYourTasteVM; }
      private set
      {
        _pasteYourTasteVM = value;
        NotifyOfPropertyChange(() => PasteYourTasteVM);
      }
    }
    private PasteYourTasteViewModel _pasteYourTasteVM;

    /// <summary>
    /// ViewModel for the <see cref="CSVDownloaderView"/>.
    /// </summary>
    public CSVDownloaderViewModel CSVDownloaderVM
    {
      get { return _csvDownloaderVM; }
      private set
      {
        _csvDownloaderVM = value;
        NotifyOfPropertyChange(() => CSVDownloaderVM);
      }
    }
    private CSVDownloaderViewModel _csvDownloaderVM;

    /// <summary>
    /// ViewModel for the <see cref="CollageCreatorView"/>
    /// </summary>
    public CollageCreatorViewModel CollageCreatorVM
    {
      get { return _collageCreatorVM; }
      private set
      {
        _collageCreatorVM = value;
        NotifyOfPropertyChange(() => CollageCreatorVM);
      }
    }
    private CollageCreatorViewModel _collageCreatorVM;

    #endregion ExtraViewModels

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

    /// <summary>
    /// Window manager used to display dialogs etc.
    /// </summary>
    private IWindowManager _windowManager;

    #endregion Private Member

    /// <summary>
    /// Constructor.
    /// </summary>
    public MainViewModel(IWindowManager windowManager)
    {
      _windowManager = windowManager;
      TitleString = "Last.fm Scrubbler WPF " + Assembly.GetExecutingAssembly().GetName().Version;
      CreateNewClient();
      SetupViewModels();
      CurrentStatus = "Waiting to scrobble...";
    }

    /// <summary>
    /// Creates the ViewModels.
    /// </summary>
    private void SetupViewModels()
    {
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
      SpotifyScrobbleVM = new SpotifyScrobbleViewModel();
      SpotifyScrobbleVM.StatusUpdated += StatusUpdated;
      SetlistFMScrobbleVM = new SetlistFMScrobbleViewModel();
      SetlistFMScrobbleVM.StatusUpdated += StatusUpdated;
      CacheScrobblerVM = new CacheScrobblerViewModel();
      CacheScrobblerVM.StatusUpdated += StatusUpdated;

      PasteYourTasteVM = new PasteYourTasteViewModel();
      PasteYourTasteVM.StatusUpdated += StatusUpdated;
      CSVDownloaderVM = new CSVDownloaderViewModel();
      CSVDownloaderVM.StatusUpdated += StatusUpdated;
      CollageCreatorVM = new CollageCreatorViewModel();
      CollageCreatorVM.StatusUpdated += StatusUpdated;
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

      if (Client.Auth.UserSession != null)
      {
        string dbFile = Client.Auth.UserSession.Username + ".db";

        try
        {
          if (!File.Exists(dbFile))
            File.Create(dbFile);
        }
        catch (Exception ex)
        {
          CurrentStatus = "Error creating cache database. Error: " + ex.Message;
        }

        Scrobbler = new Scrobbler(Client.Auth);
        CachingScrobbler = new SQLiteScrobbler(Client.Auth, dbFile);
      }
      else
      {
        Scrobbler = null;
        CachingScrobbler = null;
      }
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
    /// Shows the <see cref="UserView"/>.
    /// </summary>
    public void HyperlinkClicked()
    {
      _windowManager.ShowDialog(UserViewModel);
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