using Caliburn.Micro;
using Last.fm_Scrubbler_WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
	class UserViewModel : PropertyChangedBase
	{
		#region Properties

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

		#endregion Properties

		/// <summary>
		/// Constructor.
		/// Deserializes the users.
		/// </summary>
		public UserViewModel()
		{
			if (!Directory.Exists(USERSFOLDER))
				Directory.CreateDirectory(USERSFOLDER);

			DeserializeUsers();
		}

		/// <summary>
		/// Deserializes the saved users.
		/// </summary>
		private void DeserializeUsers()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(User));
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

		private void SerializeUsers()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(User));
			foreach (var usr in AvailableUsers)
			{
				using (var stringWriter = new StringWriter())
				{
					using (var writer = XmlWriter.Create(stringWriter))
					{
						serializer.Serialize(writer, usr);
					}
				}

			}
		}
	}
}