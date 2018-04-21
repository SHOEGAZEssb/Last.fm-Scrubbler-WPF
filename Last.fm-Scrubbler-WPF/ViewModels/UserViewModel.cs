using Caliburn.Micro;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using Scrubbler.Interfaces;
using Scrubbler.Models;
using Scrubbler.Properties;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Scrubbler.ViewModels
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
    /// Currently active user.
    /// </summary>
    public User ActiveUser
    {
      get { return _activeUser; }
      private set
      {
        _activeUser = value;
        NotifyOfPropertyChange();

        if (value != null)
          Settings.Default.Username = value.Username;
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
    private IExtendedWindowManager _windowManager;

    /// <summary>
    /// FileOperator used to write to disk.
    /// </summary>
    private IFileOperator _fileOperator;

    /// <summary>
    /// Last.fm authentication object.
    /// </summary>
    private ILastAuth _lastAuth;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// Deserializes the users.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="lastAuth">Last.fm authentication object.</param>
    /// <param name="fileOperator">FileOperator used to write to disk.</param>
    /// <param name="directoryOperator">DirectoryOperator used to check and create directories.</param>
    public UserViewModel(IExtendedWindowManager windowManager, ILastAuth lastAuth, IFileOperator fileOperator, IDirectoryOperator directoryOperator)
    {
      _windowManager = windowManager;
      _lastAuth = lastAuth;
      _fileOperator = fileOperator;
      AvailableUsers = new ObservableCollection<User>();

      if (!directoryOperator.Exists(USERSFOLDER))
        directoryOperator.CreateDirectory(USERSFOLDER);

      DeserializeUsers();
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
        _fileOperator.Delete(Path.Combine(USERSFOLDER, SelectedUser.Username) + ".xml");
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
      XmlSerializer serializer = new XmlSerializer(typeof(User), typeof(User).GetNestedTypes());
      string[] files = null;

      try
      {
        files = Directory.GetFiles(USERSFOLDER).Where(i => i.EndsWith("xml")).ToArray();
      }
      catch(Exception ex)
      {
        Debug.WriteLine("Fatal error while deserializing users: " + ex.Message);
        return;
      }

      foreach (var file in files)
      {
        try
        {
          using (StreamReader reader = new StreamReader(file))
          {
            User usr = (User)serializer.Deserialize(reader);
            AvailableUsers.Add(usr);
          }
        }
        catch (Exception ex)
        {
          Debug.WriteLine(string.Format("Error deserializing {0}: {1}", file, ex.Message));
        }
      }
    }

    /// <summary>
    /// Serializes all users in <see cref="AvailableUsers"/> to a xml file.
    /// </summary>
    private void SerializeUsers()
    {
      XmlSerializer serializer = new XmlSerializer(typeof(User));
      foreach (var usr in AvailableUsers)
      {
        try
        {
          using (var streamWriter = new StreamWriter(Path.Combine(USERSFOLDER, usr.Username) + ".xml"))
          {
            using (var writer = XmlWriter.Create(streamWriter))
            {
              serializer.Serialize(writer, usr);
            }
          }
        }
        catch (Exception ex)
        {
          Debug.WriteLine(string.Format("Error serializing User {0}: {1}", usr.Username, ex.Message));
        }
      }
    }

    /// <summary>
    /// Loads the user that was last logged in.
    /// </summary>
    public void LoadLastUser()
    {
      if (Settings.Default.Username != string.Empty)
      {
        User usr = AvailableUsers.Where(i => i.Username == Settings.Default.Username).FirstOrDefault();
        if (usr != null)
          LoginUser(usr);
      }
    }
  }
}