using IF.Lastfm.Core;
using IF.Lastfm.Core.Api;
using Newtonsoft.Json;
using Scrubbler.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
    #region Properties

    /// <summary>
    /// The name of the user to login.
    /// </summary>
    public string Username
    {
      get => _username;
      set
      {
        if(Username != value)
        {
          _username = value;
          NotifyOfPropertyChange();
        }
      }
    }
    private string _username;

    #endregion Properties

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
      _lastAuth = lastAuth ?? throw new ArgumentNullException(nameof(lastAuth));
      _messageBoxService = messageBoxService ?? throw new ArgumentNullException(nameof(messageBoxService));
    }

    /// <summary>
    /// Tries to log the user in with the given credentials.
    /// </summary>
    /// <param name="password">The <see cref="PasswordBox"/> containing the password.</param>
    public async Task Login(PasswordBox password)
    {
      try
      {
        EnableControls = false;

        if (password == null)
          throw new ArgumentNullException(nameof(password), "No password given");

        var response = await _lastAuth.GetSessionTokenAsync(Username, password.Password);

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

    public async Task LoginViaWebsite()
    {
      try
      {
        var tokenUrl = $"http://ws.audioscrobbler.com/2.0/?method=auth.gettoken&api_key={_lastAuth.ApiKey}&format=json";
        var client = new HttpClient();

        var token = string.Empty;
        // fetch token
        using (var response = await client.GetAsync(tokenUrl))
        {
          if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Could not fetch token: {response.StatusCode}");

          var tokenDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(await response.Content.ReadAsStringAsync());
          if (tokenDict.ContainsKey("token"))
            token = tokenDict["token"];
          else
            throw new Exception("Token response does not contain a token");
        }

        System.Diagnostics.Process.Start($"http://www.last.fm/api/auth/?api_key={_lastAuth.ApiKey}&token={token}");
        _messageBoxService.ShowDialog("Press OK once you have granted the Scrubbler access to your last.fm account", "Authorization");

        // fetch session
        var paramDict = new Dictionary<string, string>()
      {
        { "api_key", _lastAuth.ApiKey },
        { "method", "auth.getSession" },
        { "token", token }
      };

        var signedUrl = GetSignedURI(paramDict, true);
        using (var response = await client.GetAsync(signedUrl))
        {
          if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Could not fetch session key: {response.StatusCode}");

          var content = await response.Content.ReadAsStringAsync();
          var sessionDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(await response.Content.ReadAsStringAsync());
          if (sessionDict.ContainsKey("session"))
          {
            var sessionInfoDict = sessionDict["session"];
            if (sessionInfoDict.ContainsKey("name") && sessionInfoDict.ContainsKey("key"))
            {
              var subscriber = sessionInfoDict.ContainsKey("subscriber") ? sessionInfoDict["subscriber"] == "1" : false;
              _lastAuth.LoadSession(new IF.Lastfm.Core.Objects.LastUserSession() { Token = sessionInfoDict["key"], Username = sessionInfoDict["name"], IsSubscriber = subscriber });
              _messageBoxService.ShowDialog("Successfully logged in and authenticated!");
              TryClose(true);
            }
            else
              throw new Exception("Session info does not contain name or session key");
          }
          else
            throw new Exception("Session response does not contain a session");
        }
      }
      catch (Exception ex) 
      {
        _messageBoxService.ShowDialog("Fatal error while trying to log in via last.fm: " + ex.Message);
      }
      finally
      {
        EnableControls = true;
      }
    }

    public static string GetSignedURI(Dictionary<string, string> args, bool get)
    {
      var stringBuilder = new StringBuilder();
      if (get)
        stringBuilder.Append("http://ws.audioscrobbler.com/2.0/?");
      foreach (var kvp in args)
        stringBuilder.AppendFormat("{0}={1}&", kvp.Key, kvp.Value);
      stringBuilder.Append("api_sig=" + SignCall(args));
      stringBuilder.Append("&format=json");
      return stringBuilder.ToString();
    }

    public static string MD5(string toHash)
    {
      byte[] textBytes = Encoding.UTF8.GetBytes(toHash);
      var cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
      byte[] hash = cryptHandler.ComputeHash(textBytes);
      return hash.Aggregate("", (current, a) => current + a.ToString("x2"));
    }

    public static string SignCall(Dictionary<string, string> args)
    {
      IOrderedEnumerable<KeyValuePair<string, string>> sortedArgs = args.OrderBy(arg => arg.Key);
      string signature =
          sortedArgs.Select(pair => pair.Key + pair.Value).
          Aggregate((first, second) => first + second);
      return MD5(signature + "30a6ed8a75dad2aa6758fa607c53adb5");
    }

    /// <summary>
    /// Logs the user in if the enter key is pressed.
    /// </summary>
    /// <param name="e">EventArgs containing the pressed key.</param>
    /// <param name="password">The <see cref="PasswordBox"/> containing the password.</param>
    public async Task ButtonPressed(KeyEventArgs e, PasswordBox password)
    {
      if (e == null)
        throw new ArgumentNullException(nameof(e));

      if (e.Key == Key.Enter)
        await Login(password);
    }
  }
}