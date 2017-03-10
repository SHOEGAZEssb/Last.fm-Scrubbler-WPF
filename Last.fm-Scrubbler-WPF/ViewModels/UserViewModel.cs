using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Models;
using Last.fm_Scrubbler_WPF.Properties;
using Last.fm_Scrubbler_WPF.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for adding / removing / logging in users.
  /// </summary>
  class UserViewModel : PropertyChangedBase
  {
    #region Properties

    public event EventHandler ActiveUserChanged;

    /// <summary>
    /// The name of the folder the user xmls are saved in.
    /// </summary>
    private const string USERSFOLDER = "Users";

    /// <summary>
    /// Currently active user.
    /// </summary>
    public User ActiveUser
    {
      get { return _activeUser; }
      private set
      {
        _activeUser = value;
        NotifyOfPropertyChange(() => ActiveUser);

        if (value != null)
          Settings.Default.Username = value.Username;
        else
        {
          Settings.Default.Username = string.Empty;
          MainViewModel.CreateNewClient();
        }

        Settings.Default.Save();
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
        NotifyOfPropertyChange(() => AvailableUsers);
      }
    }
    private ObservableCollection<User> _availableUsers;

    /// <summary>
    /// Currently selected user in the listbox.
    /// </summary>
    public User SelectedUser
    {
      get { return _selectedUser; }
      set
      {
        _selectedUser = value;
        NotifyOfPropertyChange(() => SelectedUser);
        NotifyOfPropertyChange(() => CanInteractWithUser);
      }
    }
    private User _selectedUser;

    /// <summary>
    /// Gets if certain ui elements are enabled.
    /// </summary>
    public bool CanInteractWithUser
    {
      get { return SelectedUser != null; }
    }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// Deserializes the users.
    /// </summary>
    public UserViewModel()
    {
      AvailableUsers = new ObservableCollection<User>();

      if (!Directory.Exists(USERSFOLDER))
        Directory.CreateDirectory(USERSFOLDER);

      DeserializeUsers();
    }

    /// <summary>
    /// Add a user to the list.
    /// </summary>
    public void AddUser()
    {
      LoginView lv = new LoginView();
      lv.DataContext = new LoginViewModel();
      if (lv.ShowDialog().Value)
      {
        User usr = new User(MainViewModel.Client.Auth.UserSession.Username, MainViewModel.Client.Auth.UserSession.Token, MainViewModel.Client.Auth.UserSession.IsSubscriber);
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
        File.Delete(USERSFOLDER + "\\" + SelectedUser.Username + ".xml");
        AvailableUsers.Remove(SelectedUser);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Could not remove user from list: " + ex.Message);
      }
    }

    /// <summary>
    /// Logs the selected user in.
    /// </summary>
    public void LoginUser()
    {
      LoginUser(SelectedUser);
      if (MainViewModel.Client.Auth.Authenticated)
        MessageBox.Show("Successfully switched user.");
      else
        MessageBox.Show("Could not switch user. Try removing and adding the user again.");
    }

    /// <summary>
    /// Logs the given <paramref name="usr"/> in.
    /// </summary>
    /// <param name="usr">User to log in.</param>
    private void LoginUser(User usr)
    {
      var session = new LastUserSession();
      session.Username = usr.Username;
      session.Token = usr.Token;
      session.IsSubscriber = usr.IsSubscriber;

      if (MainViewModel.Client.Auth.LoadSession(session))
        ActiveUser = usr;
    }

    /// <summary>
    /// Deserializes the saved users.
    /// </summary>
    private void DeserializeUsers()
    {
      XmlSerializer serializer = new XmlSerializer(typeof(User), typeof(User).GetNestedTypes());
      var files = Directory.GetFiles(USERSFOLDER).Where(i => i.EndsWith("xml"));

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
          using (var streamWriter = new StreamWriter(USERSFOLDER + "\\" + usr.Username + ".xml"))
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
      if (Settings.Default.Username != "")
      {
        User usr = AvailableUsers.Where(i => i.Username == Settings.Default.Username).FirstOrDefault();
        if (usr != null)
          LoginUser(usr);
      }
    }
  }
}