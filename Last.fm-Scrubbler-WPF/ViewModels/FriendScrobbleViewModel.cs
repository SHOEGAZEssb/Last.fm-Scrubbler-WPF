using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  class FriendScrobbleViewModel : PropertyChangedBase
	{
		#region Properties

		/// <summary>
		/// Event that triggers when the status should be changed.
		/// </summary>
		public event EventHandler<UpdateStatusEventArgs> StatusUpdated;

		/// <summary>
		/// The username of the user to fetch scrobbles from.
		/// </summary>
		public string Username
		{
			get { return _username; }
			set
			{
				_username = value;
				NotifyOfPropertyChange(() => Username);
				NotifyOfPropertyChange(() => CanFetch);
			}
		}
		private string _username;

		/// <summary>
		/// The amount of scrobbles to be fetched.
		/// </summary>
		public int Amount
		{
			get { return _amount; }
			set
			{
				_amount = value;
				NotifyOfPropertyChange(() => Amount);
			}
		}
		private int _amount;

		/// <summary>
		/// List of fetched scrobbles.
		/// </summary>
		public ObservableCollection<FetchedScrobbleViewModel> FetchedScrobbles
		{
			get { return _fetchedScrobbles; }
			private set
			{
				_fetchedScrobbles = value;
				NotifyOfPropertyChange(() => FetchedScrobbles);
			}
		}
		private ObservableCollection<FetchedScrobbleViewModel> _fetchedScrobbles;

		/// <summary>
		/// Gets/sets if certain controls on the UI should be enabled.
		/// </summary>
		public bool EnableControls
		{
			get { return _enableControls; }
			private set
			{
				_enableControls = value;
				NotifyOfPropertyChange(() => EnableControls);
				NotifyOfPropertyChange(() => CanScrobble);
				NotifyOfPropertyChange(() => CanFetch);
			}
		}
		private bool _enableControls;

		/// <summary>
		/// Gets if the scrobble button is enabled.
		/// </summary>
		public bool CanScrobble
		{
			get { return MainViewModel.Client.Auth.Authenticated && FetchedScrobbles.Any(i => i.ToScrobble) && EnableControls; }
		}

		/// <summary>
		/// Gets if the fetch button is enabled.
		/// </summary>
		public bool CanFetch
		{
			get { return Username.Length > 0 && EnableControls; }
		}

		#endregion Properties

		/// <summary>
		/// Constructor.
		/// </summary>
		public FriendScrobbleViewModel()
		{
			EnableControls = true;
			Username = "";
			FetchedScrobbles = new ObservableCollection<FetchedScrobbleViewModel>();
			Amount = 20;
		}

		/// <summary>
		/// Fetches the recent scrobbles of the user with the given <see cref="Username"/>.
		/// </summary>
		public async void FetchScrobbles()
		{
			EnableControls = false;
			StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Trying to fetch scrobbles of user " + Username));
			FetchedScrobbles.Clear();
			var response = await MainViewModel.Client.User.GetRecentScrobbles(Username, null, 1, Amount);
			if(response.Success)
			{
				StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Successfully fetched scrobbles of user " + Username));
				foreach (var s in response)
				{
					FetchedScrobbleViewModel vm = new FetchedScrobbleViewModel(s);
					vm.ToScrobbleChanged += ToScrobbleChanged;
					FetchedScrobbles.Add(vm);
				}
			}
			else
				StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Failed to fetch scrobbles of user " + Username));

			EnableControls = true;
		}

		private void ToScrobbleChanged(object sender, EventArgs e)
		{
			NotifyOfPropertyChange(() => CanScrobble);
		}

		/// <summary>
		/// Scrobbles the selected tracks.
		/// </summary>
		public async void Scrobble()
		{
			EnableControls = false;
			StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Trying to scrobble selected tracks"));
			List<Scrobble> scrobbles = new List<Scrobble>();
			foreach(var vm in FetchedScrobbles.Where(i => i.ToScrobble))
			{
				scrobbles.Add(new Scrobble(vm.Scrobble.ArtistName, vm.Scrobble.AlbumName, vm.Scrobble.Name, vm.Scrobble.TimePlayed.Value.LocalDateTime.AddSeconds(1)));
			}

			var response = await MainViewModel.Scrobbler.ScrobbleAsync(scrobbles);
			if (response.Success)
				StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Successfully scrobbled!"));
			else
				StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Error while scrobbling!"));

			EnableControls = true;
		}
	}
}