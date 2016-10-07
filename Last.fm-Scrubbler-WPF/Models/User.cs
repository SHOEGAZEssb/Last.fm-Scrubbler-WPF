using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF.Models
{
	class User
	{
		#region Properties

		/// <summary>
		/// Username of this user.
		/// </summary>
		public string Username
		{
			get { return _username; }
			private set { _username = value; }
		}
		private string _username;

		/// <summary>
		/// Login token of this username.
		/// </summary>
		public string Token
		{
			get { return _token; }
			private set { _token = value; }
		}
		private string _token;

		#endregion Properties

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="username">Username of the user.</param>
		/// <param name="token">Login token.</param>
		public User(string username, string token)
		{
			Username = username;
			Token = token;
		}
	}
}