using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
	class ManualScrobbleViewModel : PropertyChangedBase
	{
		#region Properties

		public event EventHandler<UpdateStatusEventArgs> StatusUpdated;

		public string Artist
		{
			get { return _artist; }
			set
			{
				_artist = value;
				NotifyOfPropertyChange(() => Artist);
				NotifyOfPropertyChange(() => CanScrobble);
			}
		}
		private string _artist;

		public string Track
		{
			get { return _track; }
			set
			{
				_track = value;
				NotifyOfPropertyChange(() => Track);
				NotifyOfPropertyChange(() => CanScrobble);
			}
		}
		private string _track;

		public string Album
		{
			get { return _album; }
			set
			{
				_album = value;
				NotifyOfPropertyChange(() => Album);
			}
		}
		private string _album;

		public DateTime TimePlayed
		{
			get { return _timePlayed; }
			set
			{
				_timePlayed = value;
				NotifyOfPropertyChange(() => TimePlayed);
			}
		}
		private DateTime _timePlayed;

		public bool CurrentDateTime
		{
			get { return _currentDateTime; }
			set
			{
				_currentDateTime = value;
				if(value)
					TimePlayed = DateTime.Now;

				NotifyOfPropertyChange(() => CurrentDateTime);
			}
		}
		private bool _currentDateTime;

		public bool CanScrobble
		{
			get { return MainViewModel.Client.Auth.Authenticated && Artist.Length > 0 && Track.Length > 0; }
		}

		#endregion Properties

		/// <summary>
		/// Constructor.
		/// </summary>
		public ManualScrobbleViewModel()
		{
			Artist = "";
			Track = "";
			Album = "";
			MainViewModel.ClientAuthChanged += MainViewModel_ClientAuthChanged;
			CurrentDateTime = true;
		}

		/// <summary>
		/// Notifies the <see cref="ManualScrobbleView"/> that the <see cref="MainViewModel.Client.Auth"/>
		/// has changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainViewModel_ClientAuthChanged(object sender, EventArgs e)
		{
			NotifyOfPropertyChange(() => CanScrobble);
		}

		/// <summary>
		/// Scrobbles the track with the given info.
		/// </summary>
		public async void Scrobble()
		{
			StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Trying to scrobble..."));

			if (CurrentDateTime)
				TimePlayed = DateTime.Now;

			Scrobble s = new Scrobble(Artist, Album, Track, TimePlayed);
			var response = await MainViewModel.Scrobbler.ScrobbleAsync(s);
			if (response.Success)
				StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Successfully scrobbled!"));
			else
				StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Error while scrobbling!"));
		}
	}
}