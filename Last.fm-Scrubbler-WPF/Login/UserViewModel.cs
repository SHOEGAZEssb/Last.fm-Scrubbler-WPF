using Caliburn.Micro;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using Scrubbler.Helper;
using Scrubbler.Properties;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Scrubbler.Login
{
  /// <summary>
  /// ViewModel for adding / removing / logging in users.
  /// </summary>
  public class UserViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Event that triggers when the active user switches.
    /// </summary>
    public event EventHandler ActiveUserChanged;

    /// <summary>
    /// Username of the <see cref="ActiveUser"/>.
    /// </summary>
    public string Username
    {
      get { return ActiveUser?.Username; }
    }

    /// <summary>
    /// Count of recent scrobbles of the <see cref="ActiveUser"/>.
    /// </summary>
    public int RecentScrobbleCount
    {
      get { return ActiveUser?.RecentScrobbles.Count ?? 0; }
    }

    /// <summary>
    /// Currently active user.
    /// </summary>
    public User ActiveUser
    {
      get { return _activeUser; }
      private set
      {
        _activeUser = value;
        NotifyOfPropertyChange();
        NotifyOfPropertyChange(() => Username);
        NotifyOfPropertyChange(() => RecentScrobbleCount);

        if (ActiveUser != null)
          ActiveUser.RecentScrobblesChanged -= User_RecentScrobblesChanged;
        if (value != null)
        {
          Settings.Default.Username = value.Username;
          value.RecentScrobblesChanged += User_RecentScrobblesChanged;
        }
        else
          Settings.Default.Username = string.Empty;

        ActiveUserChanged?.Invoke(this, EventArgs.Empty);
      }
    }
    private User _activeUser;

    /// <summary>
    /// List of available users.
    /// </summary>
    public ObservableCollection<User> AvailableUsers
    {
      get { return _availableUsers; }
      private set
      {
        _availableUsers = value;
        NotifyOfPropertyChange();
      }
    }
    private ObservableCollection<User> _availableUsers;

    /// <summary>
    /// Currently selected user in the ListBox.
    /// </summary>
    public User SelectedUser
    {
      get { return _selectedUser; }
      set
      {
        _selectedUser = value;
        NotifyOfPropertyChange();
      }
    }
    private User _selectedUser;

    #endregion Properties

    #region Member

    /// <summary>
    /// The name of the folder the user xmls are saved in.
    /// </summary>
    private const string USERSFOLDER = "Users";

    /// <summary>
    /// WindowManager used to display dialogs.
    /// </summary>
    private readonly IExtendedWindowManager _windowManager;

    /// <summary>
    /// FileOperator used to write to disk.
    /// </summary>
    private readonly IFileOperator _fileOperator;

    /// <summary>
    /// DirectoryOperator used to check and create directories.
    /// </summary>
    private readonly IDirectoryOperator _directoryOperator;

    /// <summary>
    /// Last.fm authentication object.
    /// </summary>
    private readonly ILastAuth _lastAuth;

    /// <summary>
    /// Serializer used to serialize <see cref="User"/>.
    /// </summary>
    private readonly ISerializer _userSerializer;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// Deserializes the users.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="lastAuth">Last.fm authentication object.</param>
    /// <param name="fileOperator">FileOperator used to write to disk.</param>
    /// <param name="directoryOperator">DirectoryOperator used to check and create directories.</param>
    /// <param name="userSerializer">Serializer used to serialize <see cref="User"/>.</param>
    public UserViewModel(IExtendedWindowManager windowManager, ILastAuth lastAuth, IFileOperator fileOperator, IDirectoryOperator directoryOperator, ISerializer userSerializer)
    {
      _windowManager = windowManager;
      _lastAuth = lastAuth;
      _fileOperator = fileOperator;
      _directoryOperator = directoryOperator;
      _userSerializer = userSerializer;
      AvailableUsers = new ObservableCollection<User>();

      if (!_directoryOperator.Exists(USERSFOLDER))
        _directoryOperator.CreateDirectory(USERSFOLDER);

      DeserializeUsers();
      LoadLastUser();
    }

    /// <summary>
    /// Add a user to the list.
    /// </summary>
    public void AddUser()
    {
      if (_windowManager.ShowDialog(new LoginViewModel(_lastAuth, _windowManager.MessageBoxService)).Value)
      {
        User usr = new User(_lastAuth.UserSession.Username, _lastAuth.UserSession.Token, _lastAuth.UserSession.IsSubscriber);
        AvailableUsers.Add(usr);
        ActiveUser = usr;
        SerializeUsers();
      }
    }

    /// <summary>
    /// Removes the <see cref="SelectedUser"/> from the <see cref="AvailableUsers"/>
    /// and deleted its xml file.
    /// </summary>
    public void RemoveUser()
    {
      try
      {
        if (SelectedUser == ActiveUser)
          ActiveUser = null;

        // remove xml file
        _fileOperator.Delete($"{Path.Combine(USERSFOLDER, SelectedUser.Username)}.xml");
        AvailableUsers.Remove(SelectedUser);
      }
      catch (Exception ex)
      {
        _windowManager.MessageBoxService.ShowDialog("Could not remove user from list: " + ex.Message);
      }
    }

    /// <summary>
    /// Logs the selected user in.
    /// </summary>
    public void LoginUser()
    {
      LoginUser(SelectedUser);
      if (_lastAuth.Authenticated)
        _windowManager.MessageBoxService.ShowDialog("Successfully switched user.");
      else
        _windowManager.MessageBoxService.ShowDialog("Could not switch user. Try removing and adding the user again.");
    }

    /// <summary>
    /// Logs the given <paramref name="usr"/> in.
    /// </summary>
    /// <param name="usr">User to log in.</param>
    private void LoginUser(User usr)
    {
      var session = new LastUserSession()
      {
        Username = usr.Username,
        Token = usr.Token,
        IsSubscriber = usr.IsSubscriber
      };

      if (_lastAuth.LoadSession(session))
        ActiveUser = usr;
    }

    /// <summary>
    /// Deserializes the saved users.
    /// </summary>
    private void DeserializeUsers()
    {
      try
      {
        foreach (var file in _directoryOperator.GetFiles(USERSFOLDER).Where(i => i.EndsWith("xml")))
        {
          try
          {
            User usr = _userSerializer.Deserialize<User>(file);
            // connect and disconnect to serialize if recent scrobbles are different
            usr.RecentScrobblesChanged += User_RecentScrobblesChanged;
            usr.UpdateRecentScrobbles();
            usr.RecentScrobblesChanged -= User_RecentScrobblesChanged;
            AvailableUsers.Add(usr);
          }
          catch (Exception ex)
          {
            Debug.WriteLine($"Error deserializing {file}: {ex.Message}");
          }
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("Fatal error while deserializing users: " + ex.Message);
        return;
      }
    }

    /// <summary>
    /// Serializes all users in <see cref="AvailableUsers"/> to a xml file.
    /// </summary>
    private void SerializeUsers()
    {
      foreach (var usr in AvailableUsers)
      {
        try
        {
          SerializeUser(usr);
        }
        catch (Exception ex)
        {
          Debug.WriteLine($"Error serializing User {usr.Username}: {ex.Message}");
        }
      }
    }

    /// <summary>
    /// Serializes a single user.
    /// </summary>
    /// <param name="user">User to serialize.</param>
    private void SerializeUser(User user)
    {
      _userSerializer.Serialize(user, $"{Path.Combine(USERSFOLDER, user.Username)}.xml");
    }

    /// <summary>
    /// Loads the user that was last logged in.
    /// </summary>
    private void LoadLastUser()
    {
      if (Settings.Default.Username != string.Empty)
      {
        User usr = AvailableUsers.Where(i => i.Username == Settings.Default.Username).FirstOrDefault();
        if (usr != null)
          LoginUser(usr);
      }
    }

    /// <summary>
    /// Serializes users when their recent scrobble count changes.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void User_RecentScrobblesChanged(object sender, EventArgs e)
    {
      NotifyOfPropertyChange(() => RecentScrobbleCount);
      try
      {
        SerializeUser(sender as User);
      }
      catch(Exception ex)
      {
        Debug.WriteLine("Error serializing User {0}: {1}", (sender as User).Username, ex.Message);
      }
    }
  }
}