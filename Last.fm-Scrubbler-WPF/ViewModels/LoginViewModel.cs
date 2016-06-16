using Caliburn.Micro;
using Last.fm_Scrubbler_WPF.Properties;
using System.Windows;
using System.Windows.Controls;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.LoginView"/>
  /// </summary>
  class LoginViewModel : PropertyChangedBase
	{
		#region Properties

		/// <summary>
		/// Gets if certain controls should be enabled.
		/// </summary>
		public bool EnableControls
		{
			get { return _enableControls; }
			private set
			{
				_enableControls = value;
				NotifyOfPropertyChange(() => EnableControls);
			}
		}
		private bool _enableControls;

		#endregion Properties

		/// <summary>
		/// Constructor.
		/// </summary>
		public LoginViewModel()
		{
			EnableControls = true;
		}

		/// <summary>
		/// Tries to log the user in with the given credentials.
		/// </summary>
		/// <param name="win">The calling <see cref="Views.LoginView"/>.</param>
		/// <param name="username">The username.</param>
		/// <param name="password">The <see cref="PasswordBox"/> containing the password.</param>
		/// <param name="rememberLoginInformation">Bool determining if the login should be saved.</param>
		public async void Login(Window win, string username, PasswordBox password, bool rememberLoginInformation)
		{
			EnableControls = false;

			var response = await MainViewModel.Client.Auth.GetSessionTokenAsync(username, password.Password);

			if(MainViewModel.Client.Auth.Authenticated)
			{
				MessageBox.Show("Successfully logged in and authenticated!");

				if (rememberLoginInformation)
					SaveSession();
				else
				{
					Settings.Default.Token = "";
					Settings.Default.Username = "";
					Settings.Default.IsSubscriber = false;
					Settings.Default.Save();
				}

				win.DialogResult = true;
				win.Close();
			}
			else
			{
				MessageBox.Show("Failed to log in or authenticate!");
				EnableControls = true;
			}
		}

		/// <summary>
		/// Saves the user session.
		/// </summary>
		private void SaveSession()
		{
			Settings.Default.Token = MainViewModel.Client.Auth.UserSession.Token;
			Settings.Default.Username = MainViewModel.Client.Auth.UserSession.Username;
			Settings.Default.IsSubscriber = MainViewModel.Client.Auth.UserSession.IsSubscriber;
			Settings.Default.Save();
		}
	}
}