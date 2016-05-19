using Caliburn.Micro;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Views;

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
		public LastfmClient Client
		{
			get { return _client; }
			private set
			{
				_client = value;
				NotifyOfPropertyChange(() => Client);
			}
		}
		private LastfmClient _client;

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
			LoadLastSession();
		}

		/// <summary>
		/// Shows the <see cref="LoginView"/>.
		/// </summary>
		public void HyperlinkClicked()
		{
			LoginView lv = new LoginView();
			lv.DataContext = new LoginViewModel(Client);
			lv.ShowDialog();
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
				NotifyOfPropertyChange(() => StatusBarUsername);
				CurrentStatus = "Waiting to scrobble.";
			}
			else
				CurrentStatus = "Waiting for user login.";
		}
	}
}