using Caliburn.Micro;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
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
			Client = new LastfmClient(APIKEY, APISECRET);
			ManualScrobbleViewModel = new ManualScrobbleViewModel();
			ManualScrobbleViewModel.StatusUpdated += StatusUpdated;
			FriendScrobbleViewModel = new FriendScrobbleViewModel();
			FriendScrobbleViewModel.StatusUpdated += StatusUpdated;
			DatabaseScrobbleViewModel = new DatabaseScrobbleViewModel();
			DatabaseScrobbleViewModel.StatusUpdated += StatusUpdated;
			CSVScrobbleViewModel = new CSVScrobbleViewModel();
			CSVScrobbleViewModel.StatusUpdated += StatusUpdated;
			LoadLastSession();
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
			LoginView lv = new LoginView();
			lv.DataContext = new LoginViewModel();
			if ((bool)lv.ShowDialog())
				Scrobbler = new Scrobbler(Client.Auth);

			NotifyOfPropertyChange(() => StatusBarUsername);
		}

		/// <summary>
		/// Loads the last saved user session, if existing.
		/// </summary>
		private void LoadLastSession()
		{
			if (Properties.Settings.Default.Token != "" && Properties.Settings.Default.Username != "")
			{
				Client.Auth.LoadSession(new LastUserSession());
				Client.Auth.UserSession.Token = Properties.Settings.Default.Token;
				Client.Auth.UserSession.Username = Properties.Settings.Default.Username;
				Client.Auth.UserSession.IsSubscriber = Properties.Settings.Default.IsSubscriber;
				Scrobbler = new Scrobbler(Client.Auth);
				NotifyOfPropertyChange(() => StatusBarUsername);
				CurrentStatus = "Waiting to scrobble.";
			}
			else
				CurrentStatus = "Waiting for user login.";
		}
	}
}