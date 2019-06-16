using IF.Lastfm.Core.Api;
using Scrubbler.Helper;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Scrubbler.Login
{
  /// <summary>
  /// ViewModel for the <see cref="LoginView"/>
  /// </summary>
  public class LoginViewModel : ViewModelBase
  {
    #region Member

    /// <summary>
    /// Last.fm authentication object.
    /// </summary>
    private readonly ILastAuth _lastAuth;

    /// <summary>
    /// Service for showing MessageBoxes.
    /// </summary>
    private readonly IMessageBoxService _messageBoxService;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="lastAuth">Last.fm authentication object.</param>
    /// <param name="messageBoxService">Service for showing MessageBoxes.</param>
    public LoginViewModel(ILastAuth lastAuth, IMessageBoxService messageBoxService)
    {
      _lastAuth = lastAuth;
      _messageBoxService = messageBoxService;
    }

    /// <summary>
    /// Tries to log the user in with the given credentials.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The <see cref="PasswordBox"/> containing the password.</param>
    public async Task Login(string username, PasswordBox password)
    {
      try
      {
        EnableControls = false;
        var response = await _lastAuth.GetSessionTokenAsync(username, password.Password);

        if (response.Success && _lastAuth.Authenticated)
        {
          _messageBoxService.ShowDialog("Successfully logged in and authenticated!");
          TryClose(true);
        }
        else
          _messageBoxService.ShowDialog("Failed to log in or authenticate!");
      }
      catch (Exception ex)
      {
        _messageBoxService.ShowDialog("Fatal error while trying to log in: " + ex.Message);
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Logs the user in if the enter key is pressed.
    /// </summary>
    /// <param name="e">EventArgs containing the pressed key.</param>
    /// <param name="username">The username.</param>
    /// <param name="password">The <see cref="PasswordBox"/> containing the password.</param>
    public async void ButtonPressed(KeyEventArgs e, string username, PasswordBox password)
    {
      if (e.Key == Key.Enter)
        await Login(username, password);
    }
  }
}