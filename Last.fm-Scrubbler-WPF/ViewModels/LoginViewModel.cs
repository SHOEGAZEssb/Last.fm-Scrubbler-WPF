using Caliburn.Micro;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
    public async Task Login(Window win, string username, PasswordBox password)
    {
      EnableControls = false;

      var response = await MainViewModel.Client.Auth.GetSessionTokenAsync(username, password.Password);

      if (response.Success && MainViewModel.Client.Auth.Authenticated)
      {
        MessageBox.Show("Successfully logged in and authenticated!");
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
    /// Logs the user in if the enter key is pressed.
    /// </summary>
    /// <param name="e">EventArgs containing the pressed key.</param>
    /// <param name="win">The calling <see cref="Views.LoginView"/>.</param>
    /// <param name="username">The username.</param>
    /// <param name="password">The <see cref="PasswordBox"/> containing the password.</param>
    public async void ButtonPressed(KeyEventArgs e, Window win, string username, PasswordBox password)
    {
      if (e.Key == Key.Enter)
        await Login(win, username, password);
    }
  }
}